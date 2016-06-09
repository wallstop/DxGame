using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DXGame.Core;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NLog;

namespace Babel.Items
{
    /**
        <summary>
            VisibleItemComponents represent what is dropped from enemies in the Game. 
            They'll contain the knowledge of what actual Items they represent, and be
            responsible for attaching them to the player if and when they're picked up
        </summary>
    */

    [DataContract]
    [Serializable]
    public sealed class VisibleItemComponent : Component, IEnvironmentComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        private SpatialComponent Spatial { get; set; }

        [DataMember]
        private bool Activated { get; set; }

        [DataMember]
        private Type ItemComponentType { get; }

        public VisibleItemComponent(SpatialComponent spatial, Type itemComponentType)
        {
            Validate.IsNotNullOrDefault(spatial, this.GetFormattedNullOrDefaultMessage(spatial));
            Validate.IsNotNullOrDefault(itemComponentType,
                this.GetFormattedNullOrDefaultMessage(nameof(itemComponentType)));
            Validate.IsTrue(typeof(ItemComponent).IsAssignableFrom(itemComponentType),
                $"Expected {itemComponentType} to be an instance of {typeof(ItemComponent)}");
            Spatial = spatial;
            Activated = false;
            ItemComponentType = itemComponentType;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<EnvironmentInteractionMessage>(ValidEnvironmentInteractionFilter);
        }

        /* 
            TODO: Right now we can only pick up items if their center (or whatever this position is) is
            directly colliding with the player - not really what we want. What we're really looking for 
            is actual spatial intersection / collision, not point (Ie, pls refactor me pls pls)
        */
        public DxVector2 Position => Spatial.Center;

        public static GameObject Generate(VisibleItemComponent visibleItemComponent)
        {
            Validate.IsNotNullOrDefault(visibleItemComponent,
                $"Cannot generate a {typeof(GameObject)} from a null {typeof(VisibleItemComponent)}");
            SimpleSpriteComponent spriteAspect =
                SimpleSpriteComponent.Builder()
                    .WithAsset("Items/" + visibleItemComponent.ItemComponentType.Name)
                    .WithPosition(visibleItemComponent.Spatial)
                    .WithBoundingBox(visibleItemComponent.Spatial.Space)
                    .Build();
            PhysicsComponent gravityAspect =
                MapCollidablePhysicsComponent.Builder()
                    .WithWorldForces()
                    .WithSpatialComponent(visibleItemComponent.Spatial)
                    .Build();

            GameObject pandorasBox =
                GameObject.Builder()
                    .WithComponents(visibleItemComponent.Spatial, spriteAspect, gravityAspect, visibleItemComponent)
                    .Build();
            return pandorasBox;
        }

        public static SpatialComponent GenerateSpatial(DxVector2 position)
        {
            /* TODO: Non-hardcode these */
            DxVector2 itemSize = new DxVector2(25, 25);
            MapBoundedSpatialComponent spatialAspect = new MapBoundedSpatialComponent(position, itemSize);
            return spatialAspect;
        }

        private void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            GameObject source = environmentInteraction.Source;
            ItemManager itemManager = source.ComponentOfType<ItemManager>();
            itemManager.Attach(ItemComponentType);
        }

        private void ValidEnvironmentInteractionFilter(EnvironmentInteractionMessage environmentInteraction)
        {
            if(CheckIsRelevantEnvironmentInteraction(environmentInteraction))
            {
                Activated = true;
                HandleEnvironmentInteraction(environmentInteraction);
            }
        }

        private bool CheckIsRelevantEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
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
                Remove();
                return false;
            }
            return true;
        }

        public override void Remove()
        {
            /* TODO: Move to some centralized ManagerComponent? (This concept exists similarly elsewhere) */
            foreach(var component in Parent.Components.ToList())
            {
                if(ReferenceEquals(component, this))
                {
                    /* Make sure we don't stack overflow */
                    continue;
                }
                component.Remove();
            }
            base.Remove();
        }
    }
}