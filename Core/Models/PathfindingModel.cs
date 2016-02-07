using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Pathfinding;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using NLog;
using DXGame.Core.DataStructures;
using System.Collections;

namespace DXGame.Core.Models
{
    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        /* Small offset to make sure that our rectangles & collision checks encompass all points that they should */
        private const float FudgeFactor = 0.01f;

        public override bool ShouldSerialize => false;

        public PathfindingResult Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                LOG.Warn($"No work to do - we're either null or at our destination");
                return PathfindingResult.EmptyResult;
            }

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigableSurface.SurfaceFor(mapModel);
            var closestNode = navigationMesh.NodeQuery.Closest(target);
            if (!closestNode.HasValue)
            {
                LOG.Info($"Could not find a Node close to {target}");
                return PathfindingResult.EmptyResult;
            }

            NavigableSurface.Node targetNode = closestNode.Value;
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();

            Optional<NavigableSurface.Node> maybeOriginalNode =
                navigationMesh.NodeQuery.Closest(ClosestPlatformPoint(spatial));
            /*
                TODO: Have an initial pathfind round to snap ourselves to the nearest point. This currently does not work
                well if we are in midair or not aligned perfectly to a NavigableSurface point
            */
            if (!maybeOriginalNode.HasValue)
            {
                LOG.Warn($"Could not determine the closest node to {spatial.Space}");
                return PathfindingResult.EmptyResult;
            }
            NavigableSurface.Node originalNode = maybeOriginalNode.Value;
            NavigableSurface.Node current = originalNode;

            ExplorableMesh explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);
            Dictionary<CommandChain, DisplacementApproximator> displacementApproximators =
                Simulation.DetermineDisplacementApproximators(entity);

            Explore(entity, targetNode, explorableMesh, displacementApproximators);

            HashSet<NavigableSurface.Node> exhausted = new HashSet<NavigableSurface.Node>() { NavigableSurface.Node.EmptyNode };
            SortedSet<NavigableSurface.Node> available =
                new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                    {
                        int distanceCompare = (targetNode.Position - first.Position).MagnitudeSquared.CompareTo(
                            (targetNode.Position - second.Position).MagnitudeSquared);
                        if(distanceCompare != 0)
                        {
                            return distanceCompare;
                        }
                        return first.CompareTo(second);
                    }
                    )) {current};

            Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveledFrom =
                new Dictionary<NavigableSurface.Node, NavigableSurface.Node>();

            /* TODO: actual a star https://en.wikipedia.org/wiki/A*_search_algorithm */
            while (available.Any())
            {
                current = available.Min;
                if(current == targetNode)
                {
                    break;
                }
                exhausted.Add(current);
                available.Remove(current);

                List<Path> paths = explorableMesh.PathsFrom(current);
                foreach (Path path in paths)
                {
                    NavigableSurface.Node end = path.End;
                    if(!exhausted.Contains(end))
                    {
                        bool newNode = available.Add(end);
                        if(newNode)
                        {
                            traveledFrom[end] = current;
                        }
                    }
                }
            }

            PathfindingResult result = ReconstructPath(explorableMesh, targetNode, traveledFrom);
            return result;
        }

        private static PathfindingResult ReconstructPath(ExplorableMesh mesh, NavigableSurface.Node end, Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveledFrom)
        {
            NavigableSurface.Node current = end;
            LinkedList<ImmutablePair<TimeSpan, CommandChain>> directions =
                new LinkedList<ImmutablePair<TimeSpan, CommandChain>>();
            LinkedList<DxVector2> waypoints = new LinkedList<DxVector2>();
            waypoints.AddLast(current.Position);
            /* We can do direct equals here withour fear - our NavigableSurface.Nodes are straight up unique */
            while(traveledFrom.ContainsKey(current))
            {
                NavigableSurface.Node previousStep = traveledFrom[current];
                if(previousStep == current)
                {
                    break;
                }
                List<Path> paths = mesh.PathsFrom(previousStep);
                /* Pick the shortest (time-wise) path */
                paths.Sort(Comparer<Path>.Create((firstPath, secondPath) => firstPath.Time.CompareTo(secondPath.Time)));
                Path correctPath = paths.FirstOrDefault(path => path.End == current);
                if (correctPath == null)
                {
                    LOG.Warn(
                        $"Could not properly reconstruct the path (no link between {previousStep} and {current}. Bailing early.");
                    break;
                }
                directions.AddFirst(new ImmutablePair<TimeSpan, CommandChain>(correctPath.Time, correctPath.Directions));
                current = previousStep;
                waypoints.AddFirst(current.Position);
            }
            return new PathfindingResult(directions, waypoints);
        }

        private void Explore(GameObject entity, NavigableSurface.Node target, ExplorableMesh mesh,
            Dictionary<CommandChain, DisplacementApproximator> displacementApproximators)
        {
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();
            DxVector2 groundPoint = ClosestPlatformPoint(spatial);

            // TODO: Fix this and go to closest node instead of "starting" there
            Optional<NavigableSurface.Node> maybeStartingPoint = mesh.NavigableSurface.NodeQuery.Closest(groundPoint);
            if (!maybeStartingPoint.HasValue)
            {
                LOG.Warn($"Could not explore the mesh - there is no point near {groundPoint}");
                return;
            }

            NavigableSurface.Node startingPoint = maybeStartingPoint.Value;
            NavigableSurface.Node current = startingPoint;
            TimeSpan maxTime = TimeSpan.FromSeconds(0.010);
            Stopwatch timer = Stopwatch.StartNew();
            /* TODO: Properly populate available and exhausted based off of what we've already explored */
            SortedSet<NavigableSurface.Node> available =
                new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                    {
                        int distanceCompare = (target.Position - first.Position).MagnitudeSquared.CompareTo(
                            (target.Position - second.Position).MagnitudeSquared);
                        if(distanceCompare != 0)
                        {
                            return distanceCompare;
                        }
                        return first.CompareTo(second);
                    }
                    )) {current};

            while (timer.Elapsed < maxTime)
            {
                mesh.Exhausted.Add(current);
                if (!available.Any())
                {
                    break;
                }
                current = available.Min;
                available.Remove(current);
                if(Objects.Equals(current, NavigableSurface.Node.EmptyNode))
                {
                    continue;
                }

                List<CommandChain> availableCommandments = mesh.AvailableCommandChains(current);
                foreach (var commandChain in availableCommandments)
                {
                    DisplacementApproximator approximator = displacementApproximators[commandChain];
                    DxRectangle bounds = approximator.Bounds;
                    bounds.Height += spatial.Space.Height;
                    bounds.X += current.Position.X;
                    bounds.Y += current.Position.Y;
                    DxRectangle searchSpace = FudgeBounds(bounds);
                    List<NavigableSurface.Node> maybeReachableNodes =
                        mesh.NavigableSurface.NodeQuery.InRange(searchSpace);

                    if (!maybeReachableNodes.Any())
                    {
                        Path badPath = new Path(commandChain, TimeSpan.Zero, NavigableSurface.Node.EmptyNode);
                        mesh.AttachPath(current, badPath);
                        // No nodes? Truck on!
                        continue;
                    }

                    if (maybeReachableNodes.Count == 1 && maybeReachableNodes.Contains(current))
                    {
                        Path badPath = new Path(commandChain, TimeSpan.Zero, NavigableSurface.Node.EmptyNode);
                        mesh.AttachPath(current, badPath);
                        // Only ourself? Truck on!
                        continue;
                    }

                    foreach (NavigableSurface.Node node in maybeReachableNodes)
                    {
                        DxVector2 displacement = node.Position - current.Position;
                        /* 
                            Are we trying to move down but have a CommandChain that is moving us down? 
                            This is a quick hack to fix a bug, the real root should be investigated.
                        */
                        if (displacement.Y <= 0 && commandChain.Commandments.Contains(Commandment.MoveDown))
                        {
                            continue;
                        }
                        /* Determine the height that we will be at according to our displacement function */
                        double y = approximator.PositionalDisplacement(displacement.X);
                        /* And sanity check that we're within our bounds (could be something wacky like the jumping straight up case) */
                        /* 
                            TODO: This is a little bit off - we really want to extend the bounds by our spatial's space (maybe). 
                            However, if we do, we'll need to encode that information somewhere, somehow. So, for now, ... don't. Do any of that.
                        */
                        if (!FudgeBounds(approximator.Bounds).Contains(new DxVector2(displacement.X, y)))
                        {
                            continue;
                        }
                        /* Hey, maybe we can reach! */
                        TimeSpan time = approximator.TimeFor(displacement.X);
                        /* One last sanity check... */
                        if (time <= PathfindingConstants.MaxComplexSimulationTime)
                        {
                            if (!mesh.Exhausted.Contains(node))
                            {
                                available.Add(node);
                            }
                            /* We're GOOD BOYS */
                            Path path = new Path(commandChain, time, node);
                            /* 
                                TODO: We need to handle un-traversable paths. All we have right now are the movement vectors. 
                                We don't take into account if the movement vectors would collide us with anything - we simply
                                assume that if we can point a movement vector from one map point to another, than we can move there.
                                This is badong, and needs to be fixed (somewhere in this method) 
                            */
                            mesh.AttachPath(current, path);
                        }
                    }
                }
            }
        }

        /**
            <summary>
                Our normal positional points represent upper-left-hand-corner of a Rectangle.
                However, the NavigableSurface details Nodes whose points are on the platform - or, the bottom of an entity.
                This is a quick and dirty approximation of where we should be searching from.
            </summary>
        */
        private static DxVector2 ClosestPlatformPoint(SpatialComponent spatial)
        {
            return new DxVector2(spatial.Position.X, spatial.Position.Y + spatial.Height - FudgeFactor);
        }

        /* Slightly increase the bounds just a weeee bit so we cover things that we are perfectly interacting with */
        private static DxRectangle FudgeBounds(DxRectangle bounds)
        {
            bounds.X -= FudgeFactor;
            bounds.Y -= FudgeFactor;
            bounds.Width += FudgeFactor * 2;
            bounds.Height += FudgeFactor * 2;
            return bounds;
        }
    }
}