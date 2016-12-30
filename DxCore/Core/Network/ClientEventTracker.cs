using System.Collections.Generic;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Network
{
    public class ClientEventTracker
    {
        public SimpleRelayingCommandComponent PlayerCommand { get; }
        public ServerEventTracker ServerEventTracker { get; }

        public ClientEventTracker(ServerEventTracker serverEventTracker, SimpleRelayingCommandComponent playerCommand)
        {
            Validate.Hard.IsNotNullOrDefault(serverEventTracker,
                this.GetFormattedNullOrDefaultMessage(serverEventTracker));
            ServerEventTracker = serverEventTracker;
            Validate.Hard.IsNotNullOrDefault(playerCommand, this.GetFormattedNullOrDefaultMessage(playerCommand));
            PlayerCommand = playerCommand;
        }

        public void ReceiveEvents(List<Commandment> clientCommandments)
        {
            PlayerCommand.RelayCommands(clientCommandments);
        }
    }
}