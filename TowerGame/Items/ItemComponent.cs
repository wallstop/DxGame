using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public abstract class ItemComponent : DrawableComponent, IEnvironmentComponent
    {
        protected ItemComponent()
        {
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteractionMessage);
        }

        protected abstract void HandleEnvironmentInteractionMessage(EnvironmentInteractionMessage environmentInteraction);

        public abstract DxVector2 Position { get; }
    }
}
