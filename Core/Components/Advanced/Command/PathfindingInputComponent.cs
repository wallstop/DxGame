using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Command
{
    [Serializable]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        private LinkedList<ImmutablePair<TimeSpan, Commandment>> currentPath_;
        private bool ignoreNextFrame_ = false;
        private TimeSpan currentTimeout_;
        private TimeSpan timeOnCurrentCommandment_;
        private TimeSpan totalTime_;

        public PathfindingInputComponent()
        {
            UpdatePriority = UpdatePriority.HIGH;
            ResetState();
            MessageHandler.RegisterMessageHandler<PathFindingRequest>(HandlePathFindingRequest);
        }

        private void HandlePathFindingRequest(PathFindingRequest request)
        {
            var pathfindingModel = DxGame.Instance.Model<PathfindingModel>();
            var path = pathfindingModel.Pathfind(Parent, request.Location);
            ResetState();
            ignoreNextFrame_ = true;
            currentPath_ = path;
            totalTime_ = TimeSpan.Zero;
            currentTimeout_ = request.Timeout;
        }

        protected override void Update(DxGameTime gameTime)
        {
            if (ignoreNextFrame_)
            {
                ignoreNextFrame_ = false;
                return;
            }
            if (!currentPath_.Any())
            {
                return;
            }
            totalTime_ += gameTime.ElapsedGameTime;
            if (totalTime_ > currentTimeout_)
            {
                ResetState();
                return;
            }
            timeOnCurrentCommandment_ += gameTime.ElapsedGameTime;
            CullStaleDirections();
            ExecuteDirection();
        }

        private void CullStaleDirections()
        {
            if (!currentPath_.Any())
            {
                return;
            }

            bool onTrack = false;
            do
            {
                var currentInstruction = currentPath_.First();
                var time = currentInstruction.Key;
                if (timeOnCurrentCommandment_ < time)
                {
                    onTrack = true;
                }
                else
                {
                    timeOnCurrentCommandment_ -= time;
                    currentPath_.RemoveFirst();
                }
            } while (currentPath_.Any() && !onTrack);
        }

        private void ResetState()
        {
            ignoreNextFrame_ = false;
            currentPath_ = new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            currentTimeout_ = TimeSpan.Zero;
            totalTime_ = TimeSpan.Zero;
        }

        private void ExecuteDirection()
        {
            if (!currentPath_.Any())
            {
                return;
            }
            var firstCommand = currentPath_.First();
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            var commandMessage = new CommandMessage {Commandment = firstCommand.Value};
            Parent?.BroadcastMessage(commandMessage);
        }
    }
}