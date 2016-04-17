using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class DropThroughPlatformRequest : Message, ITargetedMessage
    {
        [DataMember]
        public UniqueId Target { get; set; }
    }
}
