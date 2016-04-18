using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Main;

namespace DXGame.TowerGame.Player
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

        public BasicAttackComponent()
        {
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

            if(lastAttacked_ + AttackCooldown > DxGame.Instance.CurrentTime.TotalGameTime)
            {
                // Attack still on cooldown
                return;
            }

            lastAttacked_ = DxGame.Instance.CurrentTime.TotalGameTime;

            AttackRequest attackRequest = new AttackRequest {Target = Parent.Id};
            attackRequest.Emit();
        }
    }
}