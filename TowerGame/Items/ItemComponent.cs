using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;

namespace DXGame.TowerGame.Items
{
    /**
        TODO: Flesh this out as more Items are implemented (better ideas of what parts are common)
    */

    [DataContract]
    [Serializable]
    public abstract class ItemComponent : Component, IEnvironmentComponent
    {
        protected ItemComponent()
        {
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
        }

        public abstract DxVector2 Position { get; }

        public override void Dispose()
        {
            foreach(var component in Parent.Components.ToList())
            {
                if(ReferenceEquals(component, this))
                {
                    /* Make sure we don't stack overflow */
                    continue;
                }
                component.Dispose();
            }
            base.Dispose();
        }

        protected abstract void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction);
    }
}