using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Entities
{
    /** 
        TODO: Change to have a more generic trigger mechanism (and maybe a more generic dropping mechanism)
    */

    /**
        <summary>
            Given a percentage chance (in terms of success, ie .25 will result in a 25% chance of this triggering), 
            and a function to generate Item(s), this will generate the items the correct percentage of the time 
            in response to entity death
        </summary>
    */

    [DataContract]
    [Serializable]
    [ProtoContract]
    public class ItemDropperComponent : Component
    {
        [DataMember]
        [ProtoMember(1)]
        private double PercentChance { get; }

        [DataMember]
        [ProtoMember(2)]
        private Func<DxVector2, List<GameObject>> ItemProduction { get; }

        public ItemDropperComponent(double percentChance, Func<DxVector2, List<GameObject>> itemProduction)
        {
            Validate.IsInClosedInterval(percentChance, 0, 1,
                $"Cannot create a {typeof(ItemDropperComponent)} with a {nameof(percentChance)} of {percentChance}");
            PercentChance = percentChance;
            Validate.IsNotNullOrDefault(itemProduction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(itemProduction)));
            ItemProduction = itemProduction;
            MessageHandler.RegisterMessageHandler<EntityDeathMessage>(HandleEntityDeath);
        }

        protected virtual void HandleEntityDeath(EntityDeathMessage deathMessage)
        {
            double rollTheDice = ThreadLocalRandom.Current.NextDouble();
            if(rollTheDice >= PercentChance)
            {
                return;
            }

            PositionalComponent entityPosition = Parent.ComponentOfType<PositionalComponent>();
            DxVector2 position = entityPosition.Position;

            List<GameObject> spawnedItems = ItemProduction.Invoke(position);
            DxGame.Instance.AddAndInitializeGameObjects(spawnedItems);
        }
    }
}