using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;

namespace DxCore.Core.Components.Advanced.Impulse
{
    [Serializable]
    [DataContract]
    public class MapPlatformDropper : Component
    {
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