using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Network
{
    [Serializable]
    [DataContract]
    public class ClientConnectionRequest : NetworkMessage
    {
        [DataMember]
        public string PlayerName { get; private set; }

        public ClientConnectionRequest(string playerName)
        {
            Validate.IsNotEmpty(playerName);
            PlayerName = playerName;
        }
    }
}