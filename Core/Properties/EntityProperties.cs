using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Properties
{
    [Serializable]
    [DataContract]
    public sealed class EntityProperties : JsonPersistable<EntityProperties>
    {
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
        public Property<int> AttackSpeed { get; private set; }

        [DataMember]
        public Property<int> AttackDamage { get; private set; }

        public EntityProperties(Property<int> health = null, Property<int> maxHealth = null,
            Property<int> defense = null, Property<float> moveSpeed = null, Property<float> jumpSpeed = null,
            Property<int> attackSpeed = null, Property<int> attackDamage = null)
        {
            Health = health ?? new Property<int>(1, nameof(Health));
            MaxHealth = maxHealth ?? new Property<int>(1, nameof(MaxHealth));
            Defense = defense ?? new Property<int>(1, nameof(Defense));
            MoveSpeed = moveSpeed ?? new Property<float>(1.0f, nameof(MoveSpeed));
            JumpSpeed = jumpSpeed ?? new Property<float>(1.0f, nameof(JumpSpeed));
            AttackSpeed = attackSpeed ?? new Property<int>(10, nameof(AttackSpeed));
            AttackDamage = attackDamage ?? new Property<int>(5, nameof(AttackDamage));

            MaxHealth.AttachListener(MaxHealthMutatingCurrentHealthListener);
            Health.AttachListener(HealthNeverExceedsMaxHealthListener);
        }

        /* Ensures that the current Health value never exceeds that of MaxHealth */

        private void HealthNeverExceedsMaxHealthListener(int previousHealth, int currentHealth)
        {
            if(currentHealth <= MaxHealth.CurrentValue)
            {
                return;
            }
            Health.CurrentValue = MaxHealth.CurrentValue;
        }

        /* Ensures that if MaxHealth grows / shrinks, then the current health values grows / shrinks along with it */

        private void MaxHealthMutatingCurrentHealthListener(int previousMax, int currentMax)
        {
            /* Values the same? Why were we notified? :( */
            if(previousMax == currentMax)
            {
                return;
            }

            double growth = (double) currentMax / previousMax;
            Health.CurrentValue = (int) Math.Round(Health.CurrentValue * growth);
        }
    }
}