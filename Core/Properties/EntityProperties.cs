using NLog;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Properties
{
    [Serializable]
    [DataContract]
    public sealed class EntityProperties : JsonPersistable<EntityProperties>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public override string Extension => ".prop";
        public override EntityProperties Item => this;

        /* TODO: Move all Properties to their actual types (AttackSpeed instead of double, Health instead of int, etc */
        [DataMember]
        public Property<int> Health { get; private set; }

        [DataMember]
        public Property<int> MaxHealth { get; private set; }

        [DataMember]
        public Property<int> Defense { get; private set; }

        [DataMember]
        public Property<float> MoveSpeed { get; private set; }

        [DataMember]
        public Property<float> JumpSpeed { get; private set; }

        [DataMember]
        public Property<TimeSpan> AttackSpeed { get; private set; }

        public EntityProperties(Property<int> health = null, Property<int> maxHealth = null, Property<int> defense = null, Property<float> moveSpeed = null, Property<float> jumpSpeed = null, Property<TimeSpan> attackSpeed = null)
        {
            Health = health ?? new Property<int>(1, "Health");
            MaxHealth = maxHealth ?? new Property<int>(1, "MaxHealth");
            Defense = defense ?? new Property<int>(1, "Defense");
            MoveSpeed = moveSpeed ?? new Property<float>(1.0f, "MoveSpeed");
            JumpSpeed = jumpSpeed ?? new Property<float>(1.0f, "JumpSpeed");
            AttackSpeed = attackSpeed ?? new Property<TimeSpan>(TimeSpan.FromSeconds(10), "AttackSpeed");
        }
    }
}
