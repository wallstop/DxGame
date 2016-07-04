using System;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;

namespace Babel.Player
{
    /**
        <summary>
            
        </summary>
    */

    [DataContract]
    [Serializable]
    public class BasicAttackComponent : Component
    {
        [DataMember] private TimeSpan lastAttacked_;

        private TimeSpan AttackCooldown
        {
            get
            {
                // TODO: Come up with Attack Speed / Damage / etc types
                EntityPropertiesComponent entityProperties = Parent.ComponentOfType<EntityPropertiesComponent>();
                int attackSpeed = entityProperties.EntityProperties.AttackSpeed.CurrentValue;
                return TimeSpan.FromSeconds(60.0 / attackSpeed);
            }
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleAttackRequest);
        }

        private void HandleAttackRequest(CommandMessage commandment)
        {
            if(!Equals(commandment.Target, Parent.Id))
            {
                return;
            }

            if(commandment.Commandment != Commandment.Attack)
            {
                return;
            }

            if(lastAttacked_ + AttackCooldown > DxGame.Instance.CurrentUpdateTime.TotalGameTime)
            {
                // Attack still on cooldown
                return;
            }

            lastAttacked_ = DxGame.Instance.CurrentUpdateTime.TotalGameTime;

            AttackRequest attackRequest = new AttackRequest {Target = Parent.Id};
            attackRequest.Emit();
        }
    }
}