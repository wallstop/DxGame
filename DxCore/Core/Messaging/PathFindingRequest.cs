using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class PathFindingRequest : Message, ITargetedMessage
    {
        [DataMember]
        public DxVector2 Location { get; set; }

        [DataMember]
        public TimeSpan Timeout { get; set; }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}