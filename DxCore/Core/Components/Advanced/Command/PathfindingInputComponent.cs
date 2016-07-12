using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using NLog;

namespace DxCore.Core.Components.Advanced.Command
{
    [Serializable]
    [DataContract]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private LinkedList<DxVector2> waypoints_ = new LinkedList<DxVector2>();

        [DataMember] private TimeSpan currentTimeout_;
        [DataMember] private TimeSpan timeOnCurrentCommandment_;
        [DataMember] private TimeSpan totalTime_;

        public IEnumerable<DxVector2> WayPoints => waypoints_;

        public PathfindingInputComponent()
        {
            UpdatePriority = UpdatePriority.High;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PathfindingResponse>(HandlePathfindingResponse);
            base.OnAttach();
        }

        private void HandlePathfindingResponse(PathfindingResponse pathfindingResponse)
        {
            // TODO: Interpret& turn into commands
            Logger.Debug("Finished pathding. Response: {0}", pathfindingResponse.Path);
        }

        protected override void Update(DxGameTime gameTime) {}
    }
}