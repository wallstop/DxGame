using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced.Damage
{
    /**
        Attach to stuff you want to be able to damage. 
        Interacts with the PropertiersComponent to act as a go-between for Damage & Health values.
        TODO: Incorporate Armor/Damage Reduction (as part of damage function?)

        <summary> Allows GameObjects to receive damage. </summary>
    */

    [Serializable]
    [DataContract]
    public class DamageComponent : Component
    {
        [DataMember]
        protected EntityPropertiesComponent EntityProperties { get; }

        protected DamageComponent(EntityPropertiesComponent entityProperties)
        {
            EntityProperties = entityProperties;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<DamageMessage>(HandleDamageMessage);
        }

        private void HandleDamageMessage(DamageMessage damageMessage)
        {
            if(!Equals(damageMessage.Target, Parent.Id))
            {
                return;
            }

            var damageReceived = damageMessage.DamageCheck(damageMessage.Source, Parent);
            /* If our owner wasn't damaged, don't do anything */
            if(!damageReceived.Item1)
            {
                return;
            }

            var damageAmount = damageReceived.Item2;
            /* Othwerise, directly modify health */
            EntityProperties.EntityProperties.Health.BaseValue -= (int) Math.Ceiling(damageAmount);
        }

        public static DamageComponentBuilder Builder()
        {
            return new DamageComponentBuilder();
        }

        public class DamageComponentBuilder : IBuilder<DamageComponent>
        {
            private EntityPropertiesComponent entityProperties_;

            public DamageComponent Build()
            {
                Validate.IsNotNull(entityProperties_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, entityProperties_));
                return new DamageComponent(entityProperties_);
            }

            public DamageComponentBuilder WithEntityProprerties(EntityPropertiesComponent entityProperties)
            {
                entityProperties_ = entityProperties;
                return this;
            }
        }
    }
}