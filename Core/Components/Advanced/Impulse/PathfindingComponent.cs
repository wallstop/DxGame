using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    public class PathfindingComponent : Component
    {
        [DataMember]
        public StandardActionComponent ActionComponent { get; }

        public PathfindingComponent(StandardActionComponent standardActionComponent)
        {
            Validate.IsNotNullOrDefault(standardActionComponent,
                StringUtils.GetFormattedNullOrDefaultMessage(this, standardActionComponent));
            ActionComponent = standardActionComponent;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var mapModel = DxGame.Instance.Model<MapModel>();
            var targetArea = mapModel.Map.RandomSpawnLocation;
            var targetLocation = new DxVector2(targetArea.X, targetArea.Y);
            var path = FindPath(targetLocation, TimeSpan.FromSeconds(5));
        }

        public List<ImmutablePair<DxGameTime, Commandment>> FindPath(DxVector2 destination, TimeSpan timeout)
        {
            var beginningTime = DxGame.Instance.CurrentTime;
            var pathFindingModel = DxGame.Instance.Model<PathfindingModel>();
            var fullPath = pathFindingModel.Pathfind(Parent, destination);
            var cutoffTime = beginningTime.TotalGameTime + timeout;
            /* Cull everything past our timeout */
            fullPath.RemoveAll(pair => pair.Key.TotalGameTime > cutoffTime);
            return fullPath;
        }
    }
}