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
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleDownMovement);
        }

        public void HandleDownMovement(CommandMessage message)
        {
            if(message.Commandment == Commandment.MoveDown)
            {
                DropThroughPlatformRequest dropThroughPlatformRequest = new DropThroughPlatformRequest
                {
                    Target = Parent?.Id
                };
                dropThroughPlatformRequest.Emit();
            }
        }
    }
}