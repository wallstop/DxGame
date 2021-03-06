﻿using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class EnvironmentInteractionMessage : Message, ITargetedMessage
    {
        [DataMember]
        public GameObject Source { get; set; }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}
