using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced.Command
{
    [Serializable]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        private List<ImmutablePair<TimeSpan, Commandment>> currentPath_ =
            new List<ImmutablePair<TimeSpan, Commandment>>();

        private TimeSpan timeOnCurrentCommandment_ = TimeSpan.Zero;
        private PathfindingComponent PathFinder { get; }

        public PathfindingInputComponent(PathfindingComponent pathFinder)
        {
            Validate.IsNotNullOrDefault(pathFinder, StringUtils.GetFormattedNullOrDefaultMessage(this, pathFinder));
            PathFinder = pathFinder;
            MessageHandler.RegisterMessageHandler<PathFindingRequest>(HandlePathFindingRequest);
        }

        private void HandlePathFindingRequest(PathFindingRequest request)
        {
            var path = PathFinder.FindPath(request.Location);
            currentPath_ = path;
            ExecuteDirection();
        }

        protected override void Update(DxGameTime gameTime)
        {
            if (!currentPath_.Any())
            {
                return;
            }
            timeOnCurrentCommandment_ += gameTime.ElapsedGameTime;
            var currentInstruction = currentPath_[0];
            if (timeOnCurrentCommandment_ >= currentInstruction.Key)
            {
                currentPath_.RemoveAt(0);
                ExecuteDirection();
            }
        }

        private void ExecuteDirection()
        {
            if (currentPath_.Any())
            {
                return;
            }
            var firstCommand = currentPath_[0];
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            var commandMessage = new CommandMessage {Commandment = firstCommand.Value};
            Parent?.BroadcastMessage(commandMessage);
        }
    }
}