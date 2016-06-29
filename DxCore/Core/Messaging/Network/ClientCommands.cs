using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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
            Validate.Hard.IsNotNull(clientCommandments, () => this.GetFormattedNullOrDefaultMessage(clientCommandments));
            ClientCommandments = clientCommandments;
        }
    }
}
