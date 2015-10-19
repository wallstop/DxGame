using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Models
{
    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        
        private static readonly float FRAME_RATE = (float)Main.DxGame.Instance.TargetFps;
        private static readonly TimeSpan FRAME_TIME_SLICE = TimeSpan.FromSeconds(FRAME_RATE);

        public PathfindingModel(DxGame game)
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
        }

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
            
            var commandmentQueue = new Queue<Commandment>();
            CommandmentProducer commandmentProducer = () => commandmentQueue.Dequeue();
            var entityCopy = CopyAndPrepGameObject(entity, commandmentProducer);

            var mapModel = DxGame.Model<MapModel>();
            var navigationMesh = NavigationMesh.MeshFor(mapModel);

            var exhaustedNodes = new HashSet<NavigationMesh.Node>();
            var attemptedCommandmentsAtEachNode = new Dictionary<NavigationMesh.Node, HashSet<Commandment>>(); 




            return null;
        }

        /* Note: This destructively modifies forces (if they dissipate) */
        private static float DetermineMaximumJumpHeight(List<Force> forces)
        {
            var initialPosition = new DxVector2(0, 0);
            var currentVelocity = new DxVector2(0, 0);
            var highestPosition = initialPosition.Y;
            var currentPosition = initialPosition;
            var currentTime = TimeSpan.FromSeconds(0);
            while (currentPosition.Y > initialPosition.Y)
            {
                var gameTime = ConstructGameTime(currentTime);
                var forceComputation = PhysicsComponent.ForceComputation(gameTime, currentPosition, currentVelocity,
                    forces);
                forces.RemoveAll(force => force.Dissipated);
                currentPosition = forceComputation.Item1;
                currentVelocity = forceComputation.Item2;
                highestPosition = Math.Max(highestPosition, currentPosition.Y);
                currentTime += FRAME_TIME_SLICE;
            }
            return highestPosition;
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
            var pathfinderInputFeeder = new PathfindingInputComponent(DxGame.Instance, commandmentProducer);
            entityCopy.AttachComponent(pathfinderInputFeeder);
            return entityCopy;
        }

        private void SimulateOneStep(GameObject entity, DxGameTime initialTime)
        {
            var frameRate = DxGame.TargetFps;
            var frameTimeSlice = TimeSpan.FromSeconds(frameRate);
            var nextFrame = new DxGameTime(initialTime.TotalGameTime + frameTimeSlice, frameTimeSlice, initialTime.IsRunningSlowly);
            entity.Process(nextFrame);
        }
    }
}