using System;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced.Impulse
{
    [Serializable]
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
                Parent?.BroadcastMessage(dropThroughPlatformRequest);
            }
        }
    }
}