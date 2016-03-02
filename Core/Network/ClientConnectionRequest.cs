using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    [Serializable]
    [DataContract]
    public class ClientConnectionRequest : NetworkMessage
    {
        [DataMember]
        public string PlayerName { get; set; }

        public ClientConnectionRequest() {}

        public ClientConnectionRequest(string playerName)
        {
            Validate.IsNotEmpty(playerName);
            PlayerName = playerName;
        }
    }
}