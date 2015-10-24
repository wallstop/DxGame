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
        private LinkedList<Commandment []> currentPath_;
        private TimeSpan currentTimeout_;
        private TimeSpan pathTimeout_;
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
            pathTimeout_ = path.Item1;
            currentPath_ = path.Item2;
            totalTime_ = TimeSpan.Zero;
            currentTimeout_ = request.Timeout;
        }

        protected override void Update(DxGameTime gameTime)
        {
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
                if (timeOnCurrentCommandment_ < pathTimeout_)
                {
                    onTrack = true;
                }
                else
                {
                    timeOnCurrentCommandment_ -= pathTimeout_;
                    currentPath_.RemoveFirst();
                }
            } while (currentPath_.Any() && !onTrack);
        }

        private void ResetState()
        {
            currentPath_ = new LinkedList<Commandment []>();
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            currentTimeout_ = TimeSpan.Zero;
            totalTime_ = TimeSpan.Zero;
            pathTimeout_ = TimeSpan.Zero;
        }

        private void ExecuteDirection()
        {
            if (!currentPath_.Any())
            {
                return;
            }
            var commandments = currentPath_.First();
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            foreach (Commandment commandment in commandments)
            {
                var commandMessage = new CommandMessage {Commandment = commandment};
                Parent?.BroadcastMessage(commandMessage);
            }
        }
    }
}