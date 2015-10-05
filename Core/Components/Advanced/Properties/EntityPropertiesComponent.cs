﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Properties;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{
    [Serializable]
    [DataContract]
    public class EntityPropertiesComponent : Component
    {
        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        // TODO: Move all Properties to their actual types (AttackSpeed instead of double, Health instead of int, etc
        [DataMember]
        public Property<int> Health { get; protected set; }

        [DataMember]
        public Property<int> MaxHealth { get; protected set; }

        [DataMember]
        public Property<int> Defense { get; protected set; }

        [DataMember]
        public Property<float> MoveSpeed { get; protected set; }

        [DataMember]
        public Property<float> JumpSpeed { get; protected set; }

        [DataMember]
        public Property<TimeSpan> AttackSpeed { get; protected set; }

        public EntityPropertiesComponent(DxGame game)
            : base(game)
        {
        }

        public override Component Copy()
        {
            return new EntityPropertiesComponent(DxGame)
            {
                Health = new Property<int>(Health),
                MaxHealth = new Property<int>(MaxHealth),
                Defense = new Property<int>(Defense),
                MoveSpeed = new Property<float>(MoveSpeed),
                JumpSpeed = new Property<float>(JumpSpeed),
                AttackSpeed = new Property<TimeSpan>(AttackSpeed)
            };
        }

        public override void Initialize()
        {
            /* Assume base class has dealt with actually creating the Properties */
            Health.AttachListener(EntityDeathListener);
        }

        protected virtual void EntityDeathListener(int previousHealth, int currentHealth)
        {
            /* Have we received lethal damage? */
            if (currentHealth <= 0 && previousHealth > 0)
            {
                /* If so, tell everyone that we've died. */
                var entityDeathMessage = new EntityDeathMessage() {Entity = Parent};
                /* The world deserves to know. We were important. */
                DxGame.BroadcastMessage(entityDeathMessage);
                Parent?.BroadcastMessage(entityDeathMessage);
            }
        }
    }
}