using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    [ProtoContract]
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
            Parent?.BroadcastTypedMessage(attackRequest);
        }
    }
}