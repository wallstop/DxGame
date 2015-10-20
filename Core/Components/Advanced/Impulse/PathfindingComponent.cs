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
            // TODO
            var mapModel = DxGame.Instance.Model<MapModel>();
            var targetArea = mapModel.Map.RandomSpawnLocation;
            var targetLocation = new DxVector2(targetArea.X, targetArea.Y);
            var path = FindPath(targetLocation);
        }

        public List<ImmutablePair<TimeSpan, Commandment>> FindPath(DxVector2 destination)
        {
            var pathFindingModel = DxGame.Instance.Model<PathfindingModel>();
            var fullPath = pathFindingModel.Pathfind(Parent, destination);
            return fullPath;
        }
    }
}