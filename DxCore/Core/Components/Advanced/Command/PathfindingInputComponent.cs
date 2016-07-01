using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Command
{
    [Serializable]
    [DataContract]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        [DataMember] private LinkedList<DxVector2> waypoints_ = new LinkedList<DxVector2>();

        [DataMember] private TimeSpan currentTimeout_;
        [DataMember] private TimeSpan timeOnCurrentCommandment_;
        [DataMember] private TimeSpan totalTime_;

        public IEnumerable<DxVector2> WayPoints => waypoints_;

        public PathfindingInputComponent()
        {
            UpdatePriority = UpdatePriority.HIGH;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PathFindingRequest>(HandlePathFindingRequest);
            base.OnAttach();
        }

        private void HandlePathFindingRequest(PathFindingRequest request) {}

        protected override void Update(DxGameTime gameTime) {}
    }
}