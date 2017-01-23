using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Environment;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Sprite;
using DxCore.Core.Components.Advanced.Team;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using NLog;
using WallNetCore.Validate;

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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember]
        private bool Activated { get; set; }

        [DataMember]
        private Type ItemComponentType { get; set; }

        [DataMember]
        private PhysicsComponent Physics { get; set; }

        public VisibleItemComponent(PhysicsComponent physics, Type itemComponentType)
        {
            Validate.Hard.IsNotNullOrDefault(physics, () => this.GetFormattedNullOrDefaultMessage(physics));
            Validate.Hard.IsNotNullOrDefault(itemComponentType,
                () => this.GetFormattedNullOrDefaultMessage(nameof(itemComponentType)));
            Validate.Hard.IsTrue(typeof(ItemComponent).IsAssignableFrom(itemComponentType),
                () => $"Expected {itemComponentType} to be an instance of {typeof(ItemComponent)}");
            Physics = physics;
            Activated = false;
            ItemComponentType = itemComponentType;
        }

        /* 
            TODO: Right now we can only pick up items if their center (or whatever this position is) is
            directly colliding with the player - not really what we want. What we're really looking for 
            is actual spatial intersection / collision, not point (Ie, pls refactor me pls pls)
        */
        public DxVector2 Position => Physics.Center;

        public static GameObject Generate(VisibleItemComponent visibleItemComponent)
        {
            Validate.Hard.IsNotNullOrDefault(visibleItemComponent,
                () => $"Cannot generate a {typeof(GameObject)} from a null {typeof(VisibleItemComponent)}");
            SimpleSpriteComponent spriteAspect =
                SimpleSpriteComponent.Builder()
                    .WithAsset("Items/" + visibleItemComponent.ItemComponentType.Name)
                    .WithSpatial(visibleItemComponent.Physics)
                    .Build();

            GameObject pandorasBox =
                GameObject.Builder()
                    .WithComponents(visibleItemComponent.Physics, spriteAspect, visibleItemComponent)
                    .Build();
            return pandorasBox;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<EnvironmentInteractionMessage>(ValidEnvironmentInteractionFilter);
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
                Logger.Info($"{GetType()} had a double activate call, ignoring");
                Remove();
                return false;
            }
            return true;
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
    }
}