﻿using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackRequest : Message, ITargetedMessage
    {
        /* Plea for an attack */

        [DataMember]
        public UniqueId Target { get; set; }
    }
}
