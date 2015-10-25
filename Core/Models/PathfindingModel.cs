using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using MathNet.Numerics;
using NLog;

namespace DXGame.Core.Models
{
    public delegate DxVector2 DisplacementFunction(TimeSpan offset);

    public delegate float PositionalFunction(float x);

    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        internal static readonly int SIMULATION_UPPER_BOUND = 10;
        internal static readonly double SIMULATION_STEP = 0.01;
        internal static readonly TimeSpan MAX_SIMULATION_TIME = TimeSpan.FromSeconds(SIMULATION_UPPER_BOUND);
        internal static readonly TimeSpan SIMULATION_TIME_STEP = TimeSpan.FromSeconds(SIMULATION_STEP);

        internal static readonly ReadOnlyCollection<Commandment[]> AVAILABLE_COMMANDMENTS =
            new ReadOnlyCollection<Commandment[]>(new List<Commandment[]>
            {
                new [] { Commandment.None },
                new[] {Commandment.MoveUp},
                new[] {Commandment.MoveLeft},
                new[] {Commandment.MoveRight},
                new[] {Commandment.MoveDown},
                new[] {Commandment.MoveUp, Commandment.MoveLeft},
                new[] {Commandment.MoveUp, Commandment.MoveRight},
                new[] {Commandment.MoveDown, Commandment.MoveLeft},
                new[] {Commandment.MoveDown, Commandment.MoveRight}
            });

        // Only budget a partial frame for this shit
        private static readonly TimeSpan EXPLORATION_TIMEOUT = TimeSpan.FromSeconds(0.005);

        private static PathfindingResult EmptyPath => new PathfindingResult(new LinkedList<ImmutablePair<TimeSpan, Commandment[]>>(), new LinkedList<DxVector2>());

        public PathfindingResult Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if(entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if(entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                LOG.Warn($"No work to do - we're either null or at our destination");
                return EmptyPath;
            }

            var commandmentQueue = new Queue<Commandment>();
            commandmentQueue.Enqueue(Commandment.None);

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigableSurface.SurfaceFor(mapModel);
            var closestNode = navigationMesh.NodeQuery.Closest(target);
            if(!closestNode.HasValue)
            {
                LOG.Info($"Could not find a Node close to {target}");
                return EmptyPath;
            }

            var targetNode = closestNode.Value;
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();

            Optional<NavigableSurface.Node> maybeOriginalNode =
                navigationMesh.NodeQuery.Closest(new DxVector2(spatial.Space.X, spatial.Space.Y + spatial.Space.Height - 0.01f));
            if(!maybeOriginalNode.HasValue)
            {
                LOG.Warn($"Could not determine the closest node to {spatial.Space}");
                return EmptyPath;
            }
            NavigableSurface.Node originalNode = maybeOriginalNode.Value;
            NavigableSurface.Node current = originalNode;

            var explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);
            var displacementFunctions = DetermineDisplacementFunction(entity);

            Explore(entity, targetNode, explorableMesh, displacementFunctions);

            HashSet<NavigableSurface.Node> exhausted = new HashSet<NavigableSurface.Node>();
            SortedSet<NavigableSurface.Node> available = new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                        (targetNode.Position - first.Position).MagnitudeSquared.CompareTo(
                            (targetNode.Position - second.Position).MagnitudeSquared)
                    )) { current };

            Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled =
                new Dictionary<NavigableSurface.Node, NavigableSurface.Node>();

            while(current != targetNode)
            {
                if(!available.Any())
                {
                    LOG.Warn($"Could not find path between {spatial.Space} and {target}");
                    break;
                }
                NavigableSurface.Node last = current;
                current = available.Min;
                traveled[last] = current;
                available.Remove(current);
                exhausted.Add(current);
                List<Path> paths = explorableMesh.PathsFrom(current);
                foreach(Path path in paths)
                {
                    NavigableSurface.Node end = path.End;
                    if(!exhausted.Contains(end))
                    {
                        available.Add(end);
                    }
                }
            }

            PathfindingResult result = ReconstructPath(explorableMesh, originalNode,
                targetNode, traveled);
            return result;
        }

        private static PathfindingResult ReconstructPath(ExplorableMesh mesh, NavigableSurface.Node start, NavigableSurface.Node end, Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled)
        {

            NavigableSurface.Node current = start;
            LinkedList<ImmutablePair<TimeSpan, Commandment[]>> directions =
                new LinkedList<ImmutablePair<TimeSpan, Commandment[]>>();
            LinkedList<DxVector2> waypoints = new LinkedList<DxVector2>();
            waypoints.AddLast(current.Position);
            while(current != end)
            {
                List<Path> paths = mesh.PathsFrom(current);
                if(!traveled.ContainsKey(current))
                {
                    LOG.Warn($"Could not properly reconstruct the path (no link between {current} and {end}. Bailing early.");
                    break;
                }
                NavigableSurface.Node nextStep = traveled[current];
                Path correctPath = paths.FirstOrDefault(path => path.End == nextStep);
                if(correctPath == null)
                {
                    // weird
                    LOG.Warn($"Could not properly reconstruct the path (no link between {current} and {nextStep}. Bailing early.");
                    break;
                }
                directions.AddLast(new ImmutablePair<TimeSpan, Commandment[]>(correctPath.Time, correctPath.Directions));
                current = nextStep;
                waypoints.AddLast(current.Position);
            }
            return new PathfindingResult(directions, waypoints);
        }

        private static void ReachInitialNode(GameObject entity, LinkedList<ImmutablePair<TimeSpan, Commandment[]>> directions, NavigableSurface.Node start)
        {
            // TODO

        }

        private void Explore(GameObject entity, NavigableSurface.Node target, ExplorableMesh mesh,
            Dictionary<Commandment[], SimplePolynomial> displacementFunctions)
        {
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();
            DxVector2 space = spatial.Dimensions;
            DxVector2 position = spatial.Position;
            position.Y += space.Y - 0.01f;

            // TODO: Fix this and go to closest node instead of "starting" there
            Optional<NavigableSurface.Node> maybeStartingPoint = mesh.NavigableSurface.NodeQuery.Closest(position);
            if(!maybeStartingPoint.HasValue)
            {
                LOG.Warn($"Could not explore the mesh - there is no point near {position}");
                return;
            }

            NavigableSurface.Node startingPoint = maybeStartingPoint.Value;
            NavigableSurface.Node current = startingPoint;
            TimeSpan maxTime = TimeSpan.FromSeconds(0.016);
            Stopwatch timer = Stopwatch.StartNew();
            SortedSet<NavigableSurface.Node> available = new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                        (target.Position - first.Position).MagnitudeSquared.CompareTo(
                            (target.Position - second.Position).MagnitudeSquared)
                    )) { current };

            while(timer.Elapsed < maxTime)
            {
                mesh.Exhausted.Add(current);
                if(!available.Any())
                {
                    break;
                }
                current = available.Min;
                available.Remove(current);

                List<Commandment[]> availableCommandments = mesh.AvailableCommandments(current);
                foreach(var entry in displacementFunctions)
                {
                    Commandment[] commandChain = entry.Key;
                    if(!availableCommandments.Contains(commandChain))
                    {
                        continue;
                    }
                    SimplePolynomial polynomial = entry.Value;
                    DxRectangle bounds = polynomial.Bounds;
                    bounds.Height += space.Y;
                    bounds.X += current.Position.X;
                    bounds.Y += current.Position.Y;
                    DxRectangle searchSpace = FudgeBounds(bounds);
                    List<NavigableSurface.Node> maybeReachableNodes =
                        mesh.NavigableSurface.NodeQuery.InRange(searchSpace);
                    if(!maybeReachableNodes.Any())
                    {
                        // No nodes? Truck on!
                        continue;
                    }
                    if(maybeReachableNodes.Count == 1 && maybeReachableNodes.Contains(current))
                    {
                        // Only ourself? Truck on!
                        continue;
                    }

                    foreach(NavigableSurface.Node node in maybeReachableNodes)
                    {
                        DxVector2 displacement = node.Position - current.Position;
                        if(displacement.Y <= 0 && commandChain.Contains(Commandment.MoveDown))
                        {
                            continue;
                        }
                        double y = polynomial.PositionalDisplacement(displacement.X);
                        /* Should pretty much always be false */
                        if(!FudgeBounds(polynomial.Bounds).Contains(new DxVector2(displacement.X, y)))
                        {
                            continue;
                        }
                        /* Hey, maybe we can reach! */
                        TimeSpan time = polynomial.TimeFor(displacement.X);
                        /* One last sanity check... */
                        if(time <= MAX_SIMULATION_TIME  /*&& (node.Position.Y.FuzzyCompare((float)(current.Position.Y + y), 1.0f) == 0) */)
                        {
                            if(!mesh.Exhausted.Contains(node))
                            {
                                available.Add(node);
                            }
                            /* We're GOOD BOYS */
                            Path path = Path.From(commandChain, time, node);
                            // TODO: We currently don't handle gravity (woops), so we need to add some shit in here to deal with drops between platforms
                            mesh.AttachPath(current, path);
                        }
                    }
                    mesh.ExhaustCommandment(current, commandChain);
                }
            }
        }

        private static DxRectangle FudgeBounds(DxRectangle bounds)
        {
            const float offset = 0.01f;
            bounds.X -= offset;
            bounds.Y -= offset;
            bounds.Width += offset * 2;
            bounds.Height += offset * 2;
            return bounds;
        }
    }
}