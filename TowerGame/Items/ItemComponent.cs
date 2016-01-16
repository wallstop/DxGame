using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Items
{
    /**
        TODO: Flesh this out as more Items are implemented (better ideas of what parts are common)
    */

    [DataContract]
    [Serializable]
    public abstract class ItemComponent : Component, IEnvironmentComponent
    {
        [DataMember] protected SpatialComponent Spatial
        {
            get;
            set;
        }

        [DataMember] protected bool Activated { get; set; }

        protected ItemComponent(SpatialComponent spatial)
        {
            Validate.IsNotNullOrDefault(spatial, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial));
            Spatial = spatial;
            Activated = false;
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
        }

        public virtual DxVector2 Position => Spatial.Center;

        public override void Dispose()
        {
            /* TODO: Move to some centralized ManagerComponent? (This concept exists similarly elsewhere) */
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