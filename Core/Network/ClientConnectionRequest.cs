using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace DXGame.Core.Network
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class ClientConnectionRequest : NetworkMessage
    {
        [DataMember] [ProtoMember(1)] public string PlayerName = "";

        public ClientConnectionRequest()
        {
            MessageType = MessageType.CLIENT_CONNECTION_REQUEST;
        }
    }
}