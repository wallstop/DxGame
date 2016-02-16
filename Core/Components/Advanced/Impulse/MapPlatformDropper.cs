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
    public class MapPlatformDropper : Component
    {
        public MapPlatformDropper()
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleDownMovement);
        }

        public void HandleDownMovement(CommandMessage message)
        {
            if(message.Commandment == Commandment.MoveDown)
            {
                var dropThroughPlatformRequest = new DropThroughPlatformRequest();
                Parent?.BroadcastTypedMessage(dropThroughPlatformRequest);
            }
        }
    }
}