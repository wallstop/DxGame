using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    public class AttackCommandResponder : Component
    {
        public AttackCommandResponder()
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleAttackCommand);
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