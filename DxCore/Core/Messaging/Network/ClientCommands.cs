using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Network
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
