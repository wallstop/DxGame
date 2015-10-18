using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
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

            var frameRate = DxGame.TargetFps;
            var frameTimeSlice = TimeSpan.FromSeconds(frameRate);

            /* Hacky - assume some "cooldowns" to certain commandment types to aid us in path finding */
            var commandmentCooldowns = new Dictionary<Commandment, TimeSpan>()
            {
                [Commandment.MoveUp] = TimeSpan.FromSeconds(2),
                [Commandment.MoveDown] = TimeSpan.FromSeconds(1)
            };
            /* ...then fill the rest up with default cooldown (one frame) */
            foreach (Commandment commandment in Enum.GetValues(typeof (Commandment)).Cast<Commandment>().Where(commandment => !commandmentCooldowns.ContainsKey(commandment)))
            {
                commandmentCooldowns[commandment] = frameTimeSlice;
            }

            var entityCopy = CopyAndPrepGameObject(entity);

            var orderedPath = new List<ImmutablePair<DxGameTime, Commandment>>();

            // TODO

            return null;

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

        private void SimulateOneStep(GameObject entity, DxGameTime initialTime)
        {
            var frameRate = DxGame.TargetFps;
            var frameTimeSlice = TimeSpan.FromSeconds(frameRate);
            var nextFrame = new DxGameTime(initialTime.TotalGameTime + frameTimeSlice, frameTimeSlice, initialTime.IsRunningSlowly);
            entity.Process(nextFrame);
        }
    }
}