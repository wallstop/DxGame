﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;

namespace DXGame.Core.Messaging
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