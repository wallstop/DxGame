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
    sealed internal class Path
    {
        public List<Commandment> Directions { get; }
        public Optional<Commandment> Beginning { get; }
        public NavigationMesh.Node End { get; }

        private Path(List<Commandment> directions, NavigationMesh.Node end)
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

            public Path Build(NavigationMesh.Node end)
            {
                Validate.IsNotNullOrDefault(end);
                return new Path(directions_, end);
            }
        }
    }

    sealed internal class ExplorableMesh
    {
        private readonly Dictionary<NavigationMesh.Node, HashSet<Path>> paths_ = new Dictionary<NavigationMesh.Node, HashSet<Path>>();

        public ExplorableMesh(NavigationMesh mesh)
        {
            Validate.IsNotNullOrDefault(mesh);
            foreach (var node in mesh.Nodes)
            {
                paths_[node] = new HashSet<Path>();
            }
        }

        public void AttachPath(NavigationMesh.Node start, Path path)
        {
            Validate.IsNotNull(start);
            Validate.IsNotNull(path);
            paths_[start].Add(path);
        }

        public List<Path> PathsFrom(NavigationMesh.Node start)
        {
            Validate.IsNotNull(start);
            return paths_[start].ToList();
        }

        public HashSet<Commandment> AttemptedCommandments(NavigationMesh.Node start)
        {
            return new HashSet<Commandment>(paths_[start].Where(path => path.Beginning.HasValue).Select(path => path.Beginning.Value));
        }
    }

    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        
        private static readonly float FRAME_RATE = (float)DxGame.Instance.TargetFps;
        private static readonly TimeSpan FRAME_TIME_SLICE = DxGame.Instance.TargetElapsedTime;
        
        public List<ImmutablePair<DxGameTime, Commandment>> Pathfind(GameObject entity, DxVector2 destination)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }
            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<PositionalComponent>().Position == destination)
            {
                return Enumerable.Empty<ImmutablePair<DxGameTime, Commandment>>().ToList();
            }

            var actionComponent = entity.ComponentOfType<StandardActionComponent>();
            ReadOnlyDictionary<Commandment, Force> movementCommendments = actionComponent.MovementForces.Copy();


            var commandmentQueue = new Queue<Commandment>();
            commandmentQueue.Enqueue(Commandment.None);
            CommandmentProducer commandmentProducer = () => commandmentQueue.Dequeue();
            var currentEntity = CopyAndPrepGameObject(entity, commandmentProducer);

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigationMesh.MeshFor(mapModel);
            var explorableMesh = new ExplorableMesh(navigationMesh);
            var exhausted = new HashSet<NavigationMesh.Node>();
            var available =
                new SortedSet<NavigationMesh.Node>(Comparer<NavigationMesh.Node>.Create((firstNode, secondNode) =>
                {

                    var firstDistanceToGoal =
                        (destination -
                         firstNode.Position)
                            .MagnitudeSquared;
                    var secondDistanceToGoal = (destination - secondNode.Position).MagnitudeSquared;
                    return firstDistanceToGoal.CompareTo(secondDistanceToGoal);
                }));

            var snapshots = new Dictionary<NavigationMesh.Node, Tuple<GameObject, DxGameTime>>();
            var traveled = new Dictionary<NavigationMesh.Node, NavigationMesh.Node>();

            var sourceNodes = new List<NavigationMesh.Node>();
            var pathBuilder = Path.Builder();
            var currentTime = DxGame.Instance.CurrentTime;
            var isInitialPath = true;
            Path initialPath = null;

            while (true)
            {
                if (commandmentQueue.Any())
                {
                    pathBuilder.WithStep(commandmentQueue.Peek());
                }
                currentTime = SimulateOneStep(currentEntity, currentTime);
                var spatial = currentEntity.ComponentOfType<SpatialComponent>();
                var space = spatial.Space;
                var nodesInRange = navigationMesh.NodeQuery.InRange(space);
                /* Not colliding? Just try to get closer then */
                if (!nodesInRange.Any())
                {
                    var rankedCommandments = RankCommandments(spatial, movementCommendments, destination);
                    if (rankedCommandments.Any())
                    {
                        var commandmentToUse = rankedCommandments[0];
                        pathBuilder.WithStep(commandmentToUse);
                        commandmentQueue.Enqueue(commandmentToUse);
                        continue;
                    }
                    break;
                }
                /* At a node? Great! Inform our mesh of the path that we took */
                foreach (var node in nodesInRange)
                {
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
                    var rankedCommandments = RankCommandments(spatialInstance, movementCommendments, destination);
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

            return null;
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
                return
                    (destination - spatial.Center + firstForce.Summary).MagnitudeSquared.CompareTo(
                        (destination - spatial.Center + secondForce.Summary).MagnitudeSquared);
            });
            return rankedCommandments;
        }

        private static DxGameTime ConstructGameTime(TimeSpan initialTime)
        {
            var gameTime = new DxGameTime(initialTime, FRAME_TIME_SLICE, false);
            return gameTime;
        }

        /* TODO: Make a multimap */
        private static HashSet<Commandment> RetrieveCommandmentsForNode(
            Dictionary<NavigationMesh.Node, HashSet<Commandment>> attemptedCommandments,
            NavigationMesh.Node navigationNode)
        {
            if (attemptedCommandments.ContainsKey(navigationNode))
            {
                return attemptedCommandments[navigationNode];
            }

            var emptyAttemptedCommandments = new HashSet<Commandment>();
            attemptedCommandments[navigationNode] = emptyAttemptedCommandments;
            return emptyAttemptedCommandments;
        }

        private static GameObject CopyAndPrepGameObject(GameObject entity, CommandmentProducer commandmentProducer)
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
            var pathfinderInputFeeder = new PathfindingInputComponent(commandmentProducer);
            entityCopy.AttachComponent(pathfinderInputFeeder);
            return entityCopy;
        }

        private DxGameTime SimulateOneStep(GameObject entity, DxGameTime initialTime)
        {
            var frameRate = DxGame.Instance.TargetFps;
            var frameTimeSlice = TimeSpan.FromSeconds(frameRate);
            var nextFrame = new DxGameTime(initialTime.TotalGameTime + frameTimeSlice, frameTimeSlice, initialTime.IsRunningSlowly);
            var components = new SortedList<IProcessable>(entity.Components);
            entity.Process(nextFrame);
            foreach (var component in components)
            {
                component.Process(nextFrame);
            }
            return nextFrame;
        }
    }
}