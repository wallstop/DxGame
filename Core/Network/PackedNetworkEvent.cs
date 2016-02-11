using System;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    [DataContract]
    [Serializable]
    public class PackedNetworkEvent : NetworkMessage
    {
        [DataMember]
        public Message Message { get; }

        public PackedNetworkEvent(Message message)
        {
            MessageType = MessageType.ServerDataKeyFrame;
            Validate.IsNotNullOrDefault(message);
            Message = message;
        }
    }
}