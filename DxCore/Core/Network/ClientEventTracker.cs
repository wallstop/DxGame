using System.Collections.Generic;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Network
{
    public class ClientEventTracker
    {
        public ServerEventTracker ServerEventTracker { get; }

        public SimpleRelayingCommandComponent PlayerCommand { get; }

        public void ReceiveEvents(List<Commandment> clientCommandments)
        {
            PlayerCommand.RelayCommands(clientCommandments);
        }

        public ClientEventTracker(ServerEventTracker serverEventTracker, SimpleRelayingCommandComponent playerCommand)
        {
            Validate.Hard.IsNotNullOrDefault(serverEventTracker,
                StringUtils.GetFormattedNullOrDefaultMessage(this, serverEventTracker));
            ServerEventTracker = serverEventTracker;
            Validate.Hard.IsNotNullOrDefault(playerCommand, StringUtils.GetFormattedNullOrDefaultMessage(this, playerCommand));
            PlayerCommand = playerCommand;
        }
    }
}
