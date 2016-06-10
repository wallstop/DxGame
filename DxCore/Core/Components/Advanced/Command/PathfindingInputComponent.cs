using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Pathfinding;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced.Command
{
    [Serializable]
    [DataContract]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        [DataMember] private LinkedList<ImmutablePair<TimeSpan, CommandChain>> currentPath_ =
            new LinkedList<ImmutablePair<TimeSpan, CommandChain>>();

        [DataMember] private LinkedList<DxVector2> waypoints_ = new LinkedList<DxVector2>();

        [DataMember] private TimeSpan currentTimeout_;
        [DataMember] private TimeSpan timeOnCurrentCommandment_;
        [DataMember] private TimeSpan totalTime_;

        public IEnumerable<DxVector2> WayPoints => waypoints_;

        public PathfindingInputComponent()
        {
            UpdatePriority = UpdatePriority.HIGH;
            ResetState();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PathFindingRequest>(HandlePathFindingRequest);
            base.OnAttach();
        }

        private void HandlePathFindingRequest(PathFindingRequest request)
        {
            if(!Equals(request.Target, Parent.Id))
            {
                return;
            }
            var pathfindingModel = DxGame.Instance.Model<PathfindingModel>();
            var path = pathfindingModel.Pathfind(Parent, request.Location);
            ResetState();
            currentPath_ = path.Path;
            waypoints_ = path.WayPoints;
            totalTime_ = TimeSpan.Zero;
            currentTimeout_ = request.Timeout;
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(!currentPath_.Any())
            {
                return;
            }
            totalTime_ += gameTime.ElapsedGameTime;
            if(totalTime_ > currentTimeout_)
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
            if(!currentPath_.Any())
            {
                return;
            }

            bool onTrack = false;
            do
            {
                var currentInstruction = currentPath_.First();
                if(timeOnCurrentCommandment_ < currentInstruction.Key)
                {
                    onTrack = true;
                }
                else
                {
                    timeOnCurrentCommandment_ -= currentInstruction.Key;
                    currentPath_.RemoveFirst();
                    waypoints_.RemoveFirst();
                }
            } while(currentPath_.Any() && !onTrack);
        }

        private void ResetState()
        {
            if(currentPath_.Any())
            {
                currentPath_.Clear();
            }
            if(waypoints_.Any())
            {
                waypoints_.Clear();
            }
            timeOnCurrentCommandment_ = TimeSpan.Zero;
            currentTimeout_ = TimeSpan.Zero;
            totalTime_ = TimeSpan.Zero;
        }

        private void ExecuteDirection()
        {
            if(!currentPath_.Any())
            {
                return;
            }
            var commandments = currentPath_.First();
            currentTimeout_ = commandments.Key;
            foreach(Commandment commandment in commandments.Value.Commandments)
            {
                CommandMessage commandMessage = new CommandMessage(Parent?.Id) {Commandment = commandment};
                commandMessage.Emit();
            }
        }
    }
}