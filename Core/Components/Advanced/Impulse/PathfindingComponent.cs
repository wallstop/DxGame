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

        public PathfindingComponent(DxGame game, StandardActionComponent standardActionComponent)
            : base(game)
        {
            Validate.IsNotNullOrDefault(standardActionComponent,
                StringUtils.GetFormattedNullOrDefaultMessage(this, standardActionComponent));
            ActionComponent = standardActionComponent;
        }

        public List<ImmutablePair<DxGameTime, Commandment>> FindPath(DxVector2 destination, TimeSpan timeout)
        {
            var beginningTime = DxGame.CurrentTime;
            var pathFindingModel = DxGame.Model<PathfindingModel>();
            var fullPath = pathFindingModel.Pathfind(Parent, destination);
            var cutoffTime = beginningTime.TotalGameTime + timeout;
            /* Cull everything past our timeout */
            fullPath.RemoveAll(pair => pair.Key.TotalGameTime > cutoffTime);
            return fullPath;
        }
    }
}