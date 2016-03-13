using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Network;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class ClientCommands : NetworkMessage
    {
        [DataMember]
        public List<Commandment> ClientCommandments { get; private set; }

        public ClientCommands(List<Commandment> clientCommandments)
        {
            Validate.IsNotNull(clientCommandments,
                StringUtils.GetFormattedNullOrDefaultMessage(this, clientCommandments));
            ClientCommandments = clientCommandments;
        }
    }
}
