using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Messaging;

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
