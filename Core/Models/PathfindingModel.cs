using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.DataStructures;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Models
{
    /**

        <summary>
            Describes a link between two Nodes on a NavigableSurface.
            A Path is an ordered list of Commandments. The time between each commandment is assumed to be 
            constant and defined by whatever our simulation uses as a constant time frame (ie, 16ms)
        </summary>
    */

    internal sealed class Path
    {
        public List<Commandment> Directions { get; }
        public Optional<Commandment> Beginning { get; }
        public NavigableSurface.Node End { get; }

        private Path(List<Commandment> directions, NavigableSurface.Node end)
        {
            Directions = directions;
            Beginning = directions.Any() ? directions[0] : Optional<Commandment>.Empty;
            End = end;
        }

        public static PathBuilder Builder()
        {
            return new PathBuilder();
        }

        public class PathBuilder
        {
            private readonly List<Commandment> directions_ = new List<Commandment>();

            public void WithStep(Commandment commandment)
            {
                directions_.Add(commandment);
            }

            public Path Build(NavigableSurface.Node end)
            {
                Validate.IsNotNullOrDefault(end);
                return new Path(directions_, end);
            }
        }
    }

    /**
        <summary>
            Maintains the known Paths between NavigableSurfaceNodes. This is constructed on-the-fly in 
            order to do A* pathfinding (where we do not know the directional links between Nodes, but can find them)
        </summary>
    */

    internal sealed class ExplorableMesh
    {
        private readonly Dictionary<NavigableSurface.Node, HashSet<Path>> paths_ =
            new Dictionary<NavigableSurface.Node, HashSet<Path>>();

        public ExplorableMesh(NavigableSurface mesh)
        {
            Validate.IsNotNullOrDefault(mesh);
            foreach (var node in mesh.NodeQuery.Elements)
            {
                paths_[node] = new HashSet<Path>();
            }
        }

        /**
            <summary>
                "Notifies" the mesh that there is a new path available (explored), attaching it and updating the appropriate Nodes
            </summary>
        */

        public void AttachPath(NavigableSurface.Node start, Path path)
        {
            Validate.IsNotNull(start);
            Validate.IsNotNull(path);
            // Don't loop to yourself
            if (!Objects.Equals(start, path.End))
            {
                paths_[start].Add(path);
            }
        }

        public List<NavigableSurface.Node> DetermineAvailable(NavigableSurface.Node start)
        {
            var seenNodes = new HashSet<NavigableSurface.Node>();
            var unexplored = new Queue<NavigableSurface.Node>();
            unexplored.Enqueue(start);
            do
            {
                var current = unexplored.Dequeue();
                var paths = PathsFrom(start);
                foreach (var node in paths.Select(path => path.End))
                {
                    if (seenNodes.Add(node))
                    {
                        unexplored.Enqueue(node);
                    }
                }
            } while (unexplored.Any());

            seenNodes.Remove(start);
            return seenNodes.ToList();
        }

        /**
            <summary>
                Retrieves all known Paths for the provided node.
                Note: The returned List will never be null, only (potentially) empty
            </summary>
        */

        public List<Path> PathsFrom(NavigableSurface.Node start)
        {
            Validate.IsNotNull(start);
            return paths_[start].ToList();
        }

        /**

            <summary>
                Retrieves all of the Commandments that have already been attempted for paths that start at the specified Node.
                This is useful to mark Nodes as "Exhausted"
            </summary>
        */

        public HashSet<Commandment> AttemptedCommandments(NavigableSurface.Node start)
        {
            return
                new HashSet<Commandment>(
                    paths_[start] /*.Where(path => path.Beginning.HasValue)*/.Select(path => path.Beginning.Value));
        }
    }

    internal sealed class ExplorableMeshCache
    {
        private static readonly ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>> CACHE
            =
            new ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>>(
                () => new Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>());

        public static ExplorableMesh MeshFor(GameObject entity, NavigableSurface surface)
        {
            Validate.IsNotNullOrDefault(entity,
                $"Cannot retrieve an {typeof (ExplorableMesh)} for a null {nameof(entity)}");
            Validate.IsNotNullOrDefault(surface,
                $"Cannot retrieve an {typeof (ExplorableMesh)} for a null {surface.GetType()}");
            return PopulateOrRetrieveMesh(entity, surface);
        }

        private static ExplorableMesh PopulateOrRetrieveMesh(GameObject entity, NavigableSurface surface)
        {
            var cachedNavigationMesh = CACHE.Value;
            var entityId = entity.Id;
            var key = new ImmutablePair<UniqueId, NavigableSurface>(entityId, surface);
            if (cachedNavigationMesh.ContainsKey(key))
            {
                return cachedNavigationMesh[key];
            }

            var newExplorableMesh = new ExplorableMesh(surface);
            cachedNavigationMesh[key] = newExplorableMesh;
            return newExplorableMesh;
        }
    }

    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan FRAME_TIME_SLICE =
            TimeSpan.FromMilliseconds(DxGame.Instance.TargetElapsedTime.Milliseconds);

        private static readonly TimeSpan COMPUTATION_TIMEOUT = TimeSpan.FromMilliseconds(150);
            // Only budget a partial frame for this shit

        public LinkedList<ImmutablePair<TimeSpan, Commandment>> Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                return new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            }

            var actionComponent = entity.ComponentOfType<StandardActionComponent>();
            ReadOnlyDictionary<Commandment, Force> movementCommendments = actionComponent.MovementForces.Copy();

            var commandmentQueue = new Queue<Commandment>();
            commandmentQueue.Enqueue(Commandment.None);
            var currentEntity = CopyAndPrepGameObject(entity);

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigableSurface.SurfaceFor(mapModel);
            var closestNode = navigationMesh.NodeQuery.Closest(target);
            if (!closestNode.HasValue)
            {
                LOG.Info($"Could not find a Node close to {target}");
                return new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            }

            var targetNode = closestNode.Value;

            var explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);
            var exhausted = new HashSet<NavigableSurface.Node>();
            var available =
                new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create((firstNode, secondNode) =>
                {
                    var firstDistanceToGoal =
                        (targetNode.Position -
                         firstNode.Position)
                            .MagnitudeSquared;
                    var secondDistanceToGoal = (targetNode.Position - secondNode.Position).MagnitudeSquared;
                    return firstDistanceToGoal.CompareTo(secondDistanceToGoal);
                }));

            var snapshots = new Dictionary<NavigableSurface.Node, Tuple<GameObject, TimeSpan>>();
            var traveled = new Dictionary<NavigableSurface.Node, NavigableSurface.Node>();

            List<NavigableSurface.Node> sourceNodes = new List<NavigableSurface.Node>();
            var pathBuilder = Path.Builder();
            var startTime = DxGame.Instance.CurrentTime.ElapsedGameTime;
            var currentTime = startTime;
            var isInitialPath = true;
            Path initialPath = null;
            NavigableSurface.Node best = targetNode;
            var done = false;
            var timer = Stopwatch.StartNew();
            while (!done && (timer.Elapsed < COMPUTATION_TIMEOUT))
            {
                if (commandmentQueue.Any())
                {
                    pathBuilder.WithStep(commandmentQueue.Peek());
                }
                // TODO: Need to mix in exploration with... already explored...ness
                var currentCommandment = commandmentQueue.Dequeue();
                currentTime = SimulateOneStep(currentEntity, currentTime, currentCommandment);
                var spatial = currentEntity.ComponentOfType<SpatialComponent>();
                var space = spatial.Space;
                var nodesInRange = navigationMesh.NodeQuery.InRange(space);
                if (nodesInRange.Any())
                {
                    nodesInRange = nodesInRange.Except(sourceNodes).ToList();
                }
                /* Not colliding? Just try to get closer then */
                if (!nodesInRange.Any())
                {
                    // Do nothing until we have interacted with the navigable surface
                    //var commandmentToUse = Commandment.None;
                    //pathBuilder.WithStep(commandmentToUse);
                    //commandmentQueue.Enqueue(commandmentToUse);
                    var rankedCommandments = RankCommandments(spatial, movementCommendments, targetNode.Position);
                    if (rankedCommandments.Any())
                    {
                        var commandmentToUse = rankedCommandments[0];
                        pathBuilder.WithStep(commandmentToUse);
                        commandmentQueue.Enqueue(commandmentToUse);
                    }
                    continue;
                }
                /* At a node? Great! Inform our mesh of the path that we took */
                foreach (var node in nodesInRange)
                {
                    /* Are we there? Great! Mark it but update all the new paths, because why not? */
                    if (Objects.Equals(node, targetNode))
                    {
                        done = true;
                    }

                    if (!exhausted.Contains(node))
                    {
                        bool added = available.Add(node);
                        if (added)
                        {
                            snapshots[node] = Tuple.Create(currentEntity.Copy(), (currentTime - startTime));
                        }
                    }
                    var path = pathBuilder.Build(node);
                    if (isInitialPath)
                    {
                        isInitialPath = false;
                        initialPath = path;
                        sourceNodes = nodesInRange;
                    }
                    //else
                    {
                        foreach (var sourceNode in sourceNodes)
                        {
                            explorableMesh.AttachPath(sourceNode, path);
                            /* While this does get over written, we don't really care... */
                            traveled[sourceNode] = node;
                        }
                    }
                }
                /* annnd determine what we should do next! */
                pathBuilder = Path.Builder();
                sourceNodes = nodesInRange;

                do
                {
                    var availableNode = available.Min;
                    var attemptedCommandments = explorableMesh.AttemptedCommandments(availableNode);
                    var entityAtNode = snapshots[availableNode].Item1;
                    var spatialInstance = entityAtNode.ComponentOfType<SpatialComponent>();
                    var rankedCommandments = RankCommandments(spatialInstance, movementCommendments, targetNode.Position);
                    var unusedCommandments = rankedCommandments.Except(attemptedCommandments).ToList();
                    if (!unusedCommandments.Any())
                    {
                        available.Remove(availableNode);
                        exhausted.Add(availableNode);
                        continue;
                    }
                    currentEntity = entityAtNode.Copy();
                    currentTime = startTime + snapshots[availableNode].Item2;
                    var commandmentToUse = unusedCommandments[0];
                    commandmentQueue.Enqueue(commandmentToUse);
                    break;
                } while (available.Any());

                if (!commandmentQueue.Any())
                {
                    break;
                }
            }


            if (!done)
            {
                LOG.Info($"Failed to find path to {targetNode}, closest to {target}, defaulting to best-guess");
                // TRY TO DO IT ANYWAY FOOL
                best = available.Min;
            }


            if (ReferenceEquals(null, initialPath) || ReferenceEquals(null, best))
            {
                return new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            }

            var startNode = initialPath.End;

            var finalPath = RecreatePath(explorableMesh, traveled, startNode, best);
            return finalPath;
        }

        private static LinkedList<ImmutablePair<TimeSpan, Commandment>> RecreatePath(ExplorableMesh explorableMesh,
            Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled, NavigableSurface.Node start,
            NavigableSurface.Node end)
        {
            var path = new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            var currentNode = start;
            var visited = new HashSet<NavigableSurface.Node> {currentNode};
            while (!Objects.Equals(currentNode, end))
            {
                var availablePaths = explorableMesh.PathsFrom(currentNode);
                if (!traveled.ContainsKey(currentNode))
                {
                    break;
                }
                var endpoint = traveled[currentNode];
                var isNewEndpoint = visited.Add(endpoint);
                if (!isNewEndpoint)
                {
                    break;
                }
                var chosenPath =
                    availablePaths.FirstOrDefault(availablePath => Objects.Equals(availablePath.End, endpoint));
                if (ReferenceEquals(null, chosenPath))
                {
                    break;
                }
                foreach (
                    var pathSegment in
                        chosenPath.Directions.Select(
                            segment => new ImmutablePair<TimeSpan, Commandment>(FRAME_TIME_SLICE, segment)))
                {
                    path.AddLast(pathSegment);
                }
                currentNode = endpoint;
            }
            return path;
        }

        private static List<Commandment> RankCommandments(SpatialComponent spatial,
            ReadOnlyDictionary<Commandment, Force> movementCommendments, DxVector2 destination)
        {
            var rankedCommandments = new List<Commandment>();
            var space = spatial.Space;
            if (space.Contains(destination))
            {
                return rankedCommandments;
            }

            rankedCommandments.AddRange(movementCommendments.Select(entry => entry.Key));
            rankedCommandments.Sort((firstCommandment, secondCommandment) =>
            {
                var firstForce =
                    movementCommendments[firstCommandment];
                var secondForce = movementCommendments[secondCommandment];
                var comparison =
                    (destination - (spatial.Center + firstForce.Summary)).MagnitudeSquared.CompareTo(
                        (destination - (spatial.Center + secondForce.Summary)).MagnitudeSquared);
                return comparison;
            });
            return rankedCommandments;
        }

        private static GameObject CopyAndPrepGameObject(GameObject entity)
        {
            var entityCopy = entity.Copy();
            /* 
                We need to remove all AbstractCommandComponents so no Pathfinding / 
                other commands will get in the way of our simulation 
            */
            entityCopy.RemoveComponents<AbstractCommandComponent>();
            entityCopy.CurrentMessages.RemoveAll(message => message is CommandMessage);
            entityCopy.FutureMessages.RemoveAll(message => message is CommandMessage);
            return entityCopy;
        }

        private TimeSpan SimulateOneStep(GameObject entity, TimeSpan initial, Commandment commandment)
        {
            var newTime = initial + FRAME_TIME_SLICE;
            var nextFrame = new DxGameTime(newTime, FRAME_TIME_SLICE, false);
            var components = new SortedList<IProcessable>(entity.Components);
            // Cheat - just dump the message right into their queue - rely on the fact that state machines can't be triggered via immediate mode (TODO: PLS FIX)
            var commandMessage = new CommandMessage {Commandment = commandment};
            entity.FutureMessages.Add(commandMessage);
            entity.Process(nextFrame);
            foreach (var component in components)
            {
                component.Process(nextFrame);
            }
            //entity.Process(nextFrame);
            //foreach (var component in components)
            //{
            //    component.Process(nextFrame);
            //}
            return newTime;
        }
    }
}