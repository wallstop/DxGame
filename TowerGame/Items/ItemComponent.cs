using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.TowerGame.Items
{
    /**
        TODO: Flesh this out as more Items are implemented (better ideas of what parts are common)
    */

    [DataContract]
    [Serializable]
    public abstract class ItemComponent : Component, IEnvironmentComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        protected SpatialComponent Spatial { get; set; }

        [DataMember]
        protected bool Activated { get; set; }

        public static SpatialComponent GenerateSpatial(DxVector2 position)
        {
            DxVector2 itemSize = new DxVector2(25, 25);
            MapBoundedSpatialComponent spatialAspect = new MapBoundedSpatialComponent(position, itemSize);
            return spatialAspect;
        }

        public static GameObject Generate(ItemComponent itemComponent)
        {
            Validate.IsNotNullOrDefault(itemComponent,
                $"Cannot generate a {typeof(GameObject)} from a null {typeof(ItemComponent)}");
            SimpleSpriteComponent spriteAspect =
                SimpleSpriteComponent.Builder()
                    .WithAsset("Items/PandorasBox")
                    .WithPosition(itemComponent.Spatial)
                    .WithBoundingBox(itemComponent.Spatial.Space)
                    .Build();
            PhysicsComponent gravityAspect =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(itemComponent.Spatial).Build();
            
            GameObject pandorasBox =
                GameObject.Builder().WithComponents(itemComponent.Spatial, spriteAspect, gravityAspect, itemComponent).Build();
            return pandorasBox;
        }

        protected ItemComponent(SpatialComponent spatial)
        {
            Validate.IsNotNullOrDefault(spatial, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial));
            Spatial = spatial;
            Activated = false;
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
        }

        public virtual DxVector2 Position => Spatial.Center;

        protected bool CheckIsRelevantEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            GameObject source = environmentInteraction.Source;
            TeamComponent teamComponent = source.ComponentOfType<TeamComponent>();
            Team interactionTeam = teamComponent.Team;
            if(!Equals(interactionTeam, Team.PlayerTeam))
            {
                return false;
            }

            if(Activated)
            {
                LOG.Info($"{GetType()} had a double activate call, ignoring");
                Dispose();
                return false;
            }
            return true;
        }

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