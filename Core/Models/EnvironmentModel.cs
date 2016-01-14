using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.Core.Models
{
    [Serializable]
    [DataContract]
    public class EnvironmentModel : Model
    {
        public ISpatialTree<IEnvironmentComponent> Collidables { get; private set; }

        public override bool ShouldSerialize => false;

        public EnvironmentModel()
        {
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteractionMessage);
        }

        protected override void Update(DxGameTime gameTime)
        {
            List<IEnvironmentComponent> enrivonmentComponents =
                DxGame.Instance.DxGameElements.OfType<IEnvironmentComponent>().ToList();
            MapModel mapModel = DxGame.Instance.Model<MapModel>();
            Collidables = new QuadTree<IEnvironmentComponent>(environmentComponent => environmentComponent.Position,
                mapModel.MapBounds, enrivonmentComponents);
        }

        private void HandleEnvironmentInteractionMessage(EnvironmentInteractionMessage message)
        {
            DxRectangle sourceSpace = message.Source.ComponentOfType<SpatialComponent>().Space;
            foreach(IEnvironmentComponent environmentComponent in Collidables.InRange(sourceSpace))
            {
                environmentComponent.Parent?.BroadcastMessage(message);
            }
        }
    }
}