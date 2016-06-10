using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;

namespace DxCore.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    public class AttackCommandResponder : Component
    {
        public AttackCommandResponder()
        {
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleAttackCommand);
        }

        private void HandleAttackCommand(CommandMessage commandMessage)
        {
            if(!Equals(commandMessage.Target, Parent.Id))
            {
                return;
            }

            if(commandMessage.Commandment != Commandment.Attack)
            {
                return;
            }

            AttackRequest attackRequest = new AttackRequest {Target = Parent.Id};
            attackRequest.Emit();
        }
    }
}