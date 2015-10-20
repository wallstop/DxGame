using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.DataStructures;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using NetTopologySuite.Geometries;
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
    sealed internal class Path
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
    sealed internal class ExplorableMesh
    {
        private readonly Dictionary<NavigableSurface.Node, HashSet<Path>> paths_ = new Dictionary<NavigableSurface.Node, HashSet<Path>>();

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
            paths_[start].Add(path);
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
            return new HashSet<Commandment>(paths_[start].Where(path => path.Beginning.HasValue).Select(path => path.Beginning.Value));
        }
    }

    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        
        private static readonly TimeSpan FRAME_TIME_SLICE = DxGame.Instance.TargetElapsedTime;
        
        public List<ImmutablePair<TimeSpan, Commandment>> Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                return Enumerable.Empty<ImmutablePair<TimeSpan, Commandment>>().ToList();
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
                return Enumerable.Empty<ImmutablePair<TimeSpan, Commandment>>().ToList();
            }

            var targetNode = closestNode.Value;

            var explorableMesh = new ExplorableMesh(navigationMesh);
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

            var snapshots = new Dictionary<NavigableSurface.Node, Tuple<GameObject, DxGameTime>>();
            var traveled = new Dictionary<NavigableSurface.Node, NavigableSurface.Node>();

            var sourceNodes = new List<NavigableSurface.Node>();
            var pathBuilder = Path.Builder();
            var currentTime = DxGame.Instance.CurrentTime.Copy();
            var isInitialPath = true;
            Path initialPath = null;

            var done = false;
            while (!done)
            {
                if (commandmentQueue.Any())
                {
                    pathBuilder.WithStep(commandmentQueue.Peek());
                }
                var currentCommandment = commandmentQueue.Dequeue();
                currentTime = SimulateOneStep(currentEntity, currentTime, currentCommandment);
                var spatial = currentEntity.ComponentOfType<SpatialComponent>();
                var space = spatial.Space;
                var nodesInRange = navigationMesh.NodeQuery.InRange(space);
                /* Not colliding? Just try to get closer then */
                if (!nodesInRange.Any())
                {
                    var rankedCommandments = RankCommandments(spatial, movementCommendments, targetNode.Position);
                    if (rankedCommandments.Any())
                    {
                        var commandmentToUse = rankedCommandments[0];
                        pathBuilder.WithStep(commandmentToUse);
                        commandmentQueue.Enqueue(commandmentToUse);
                        continue;
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
                        available.Add(node);
                        snapshots[node] = Tuple.Create(currentEntity.Copy(), currentTime.Copy());
                    }
                    var path = pathBuilder.Build(node);
                    if (isInitialPath)
                    {
                        isInitialPath = false;
                        initialPath = pathBuilder.Build(node);
                    }
                    else
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

                foreach (var availableNode in available)
                {
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
                    currentTime = snapshots[availableNode].Item2.Copy();
                    var commandmentToUse = unusedCommandments[0];
                    commandmentQueue.Enqueue(commandmentToUse);
                    break;
                }
                if (!commandmentQueue.Any())
                {
                    break;
                }
            }

            if (!done)
            {
                LOG.Info($"Failed to find path to {targetNode}, closest to {target}");
                return Enumerable.Empty<ImmutablePair<TimeSpan, Commandment>>().ToList();
            }

            var startNode = initialPath.End;

            var finalPath = RecreatePath(explorableMesh, traveled, startNode, targetNode);
            return finalPath;
        }

        private static List<ImmutablePair<TimeSpan, Commandment>> RecreatePath(ExplorableMesh explorableMesh,
            Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled, NavigableSurface.Node start,
            NavigableSurface.Node end)
        {
            var path = new List<ImmutablePair<TimeSpan, Commandment>>();
            var currentNode = start;
            while (!Objects.Equals(start, end))
            {
                var availablePaths = explorableMesh.PathsFrom(currentNode);
                var endpoint = traveled[currentNode];
                var chosenPath = availablePaths.First(availablePath => Objects.Equals(availablePath.End, endpoint));
                path.AddRange(chosenPath.Directions.Select(segment => new ImmutablePair<TimeSpan, Commandment>(FRAME_TIME_SLICE, segment)));
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
            entityCopy.RemoveComponents<PathfindingComponent>();
            entityCopy.CurrentMessages.RemoveAll(message => message is CommandMessage);
            entityCopy.FutureMessages.RemoveAll(message => message is CommandMessage);
            return entityCopy;
        }

        private DxGameTime SimulateOneStep(GameObject entity, DxGameTime initialTime, Commandment commandment)
        {
            var nextFrame = new DxGameTime(initialTime.TotalGameTime + FRAME_TIME_SLICE, FRAME_TIME_SLICE, initialTime.IsRunningSlowly);
            var components = new SortedList<IProcessable>(entity.Components);
            entity.Process(nextFrame);
            foreach (var component in components)
            {
                component.Process(nextFrame);
            }
            var commandMessage = new CommandMessage() {Commandment = commandment};
            entity.FutureMessages.Add(commandMessage);
            return nextFrame;
        }
    }
}