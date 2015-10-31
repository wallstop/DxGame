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

            ExplorableMesh explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);
            Dictionary<CommandChain, DisplacementApproximator> displacementApproximators =
                Simulation.DetermineDisplacementApproximators(entity);

            Explore(entity, targetNode, explorableMesh, displacementApproximators);
            HashSet<Path> exhausted = new HashSet<Path>();
            HashSet<Path> availablePaths = new HashSet<Path>();
            SortedSet<PathfindingEdge> rankedAvailable = new SortedSet<PathfindingEdge>();
            Dictionary<NavigableSurface.Node, PathfindingEdge> internalGraph = new Dictionary<NavigableSurface.Node, PathfindingEdge>();

            Path initialPath = new Path(PathfindingConstants.CommandChainNone, TimeSpan.Zero, null, originalNode);
            PathfindingEdge initialEdge = new PathfindingEdge(initialPath, 0, 0);
            internalGraph[originalNode] = initialEdge;
            foreach(Path path in explorableMesh.PathsFrom(originalNode))
            {
                if(exhausted.Add(path))
                {
                    rankedAvailable.Add(new PathfindingEdge(path, AStarHeuristic(path.Start, path.End), AStarHeuristic(path.End, targetNode)));
                }
            }

            while (rankedAvailable.Any())
            {
                PathfindingEdge current = rankedAvailable.Min;
                Path definingPath = current.Path;
                if(!internalGraph.ContainsKey(definingPath.End) || internalGraph[definingPath.End].G > current.G)
                {
                    internalGraph[definingPath.End] = current;
                }
                if(definingPath.End == targetNode)
                {
                    break;
                }

                rankedAvailable.Remove(current);
                availablePaths.Remove(definingPath);
                exhausted.Add(definingPath);
                foreach(Path neighbor in explorableMesh.PathsFrom(definingPath.End))
                {
                    if(exhausted.Contains(neighbor))
                    {
                        continue;
                    }
                    float tentativeGScore = current.G + AStarHeuristic(neighbor.End, neighbor.Start);
                    PathfindingEdge neighborEdge = new PathfindingEdge(neighbor, tentativeGScore, AStarHeuristic(neighbor.End, targetNode));
                    if(!availablePaths.Add(neighbor))
                    {
                        continue;
                    }
                    availablePaths.Add(neighbor);
                    rankedAvailable.Add(neighborEdge);
                }
            }

            PathfindingResult result = ReconstructPath(explorableMesh, originalNode, targetNode, internalGraph);
            return result;
        }

        private static float AStarHeuristic(NavigableSurface.Node start, NavigableSurface.Node end)
        {
            return (start.Position - end.Position).MagnitudeSquared;
        }

        private static PathfindingResult ReconstructPath(ExplorableMesh mesh, NavigableSurface.Node start, NavigableSurface.Node end, Dictionary<NavigableSurface.Node, PathfindingEdge> traveledFrom)
        {
            NavigableSurface.Node current = end;
            LinkedList<ImmutablePair<TimeSpan, CommandChain>> directions =
                new LinkedList<ImmutablePair<TimeSpan, CommandChain>>();
            LinkedList<DxVector2> waypoints = new LinkedList<DxVector2>();
            waypoints.AddLast(current.Position);
            /* We can do direct equals here withour fear - our NavigableSurface.Nodes are straight up unique */
            while(!ReferenceEquals(current, null) && traveledFrom.ContainsKey(current))
            {
                PathfindingEdge edge = traveledFrom[current];
                Path correctPath = edge.Path;
                directions.AddFirst(new ImmutablePair<TimeSpan, CommandChain>(correctPath.Time, correctPath.Directions));
                current = correctPath.Start;
                waypoints.AddFirst(correctPath.End.Position);
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
                        // No nodes? Truck on!
                        continue;
                    }

                    if (maybeReachableNodes.Count == 1 && maybeReachableNodes.Contains(current))
                    {
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
                            Path path = new Path(commandChain, time, current, node);
                            /* 
                                TODO: We need to handle un-traversable paths. All we have right now are the movement vectors. 
                                We don't take into account if the movement vectors would collide us with anything - we simply
                                assume that if we can point a movement vector from one map point to another, than we can move there.
                                This is badong, and needs to be fixed (somewhere in this method) 
                            */
                            mesh.AttachPath(path);
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

        private class PathfindingEdge : IComparable<PathfindingEdge>
        {
            public Path Path
            {
                get;
            }

            public float G
            {
                get;
            }

            public float F
            {
                get;
            }

            public PathfindingEdge(Path path, float g, float f)
            {
                Validate.IsNotNull(path);
                Path = path;
                G = g;
                F = f;
            }

            public int CompareTo(PathfindingEdge other)
            {
                if(ReferenceEquals(other, null))
                {
                    return 1;
                }
                if(Objects.Equals(Path, other.Path))
                {
                    return 0;
                }
                int weightComparison = F.CompareTo(other.F);
                if(weightComparison != 0)
                {
                    return weightComparison;
                }
                return Path.CompareTo(other.Path);
            }

            public override int GetHashCode()
            {
                return Path.GetHashCode();
            }

            public override bool Equals(object other)
            {
                PathfindingEdge edge = other as PathfindingEdge;
                if(!ReferenceEquals(edge, null))
                {
                    return Path.Equals(edge.Path);
                }
                return false;
            }
        }
    }
}