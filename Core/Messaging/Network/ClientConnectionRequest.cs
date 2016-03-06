using System;
using System.Runtime.Serialization;
using DXGame.Core.Network;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Network
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