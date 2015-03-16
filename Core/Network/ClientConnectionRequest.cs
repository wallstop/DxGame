using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Network
{
    [Serializable]
    [DataContract]
    public class ClientConnectionRequest : NetworkMessage
    {
        [DataMember] public string PlayerName = "";

        public ClientConnectionRequest()
        {
            MessageType = MessageType.CLIENT_CONNECTION_REQUEST;
        }
    }
}