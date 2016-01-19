using System;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
    public class AttackCommandResponder : Component
    {
        public AttackCommandResponder()
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleAttackCommand);
        }

        private void HandleAttackCommand(CommandMessage commandMessage)
        {
            if(commandMessage.Commandment != Commandment.Attack)
            {
                return;
            }

            AttackRequest attackRequest = new AttackRequest();
            Parent?.BroadcastMessage<AttackRequest>(attackRequest);
        }
    }
}