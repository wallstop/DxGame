using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging.Entity
{
    public enum EntityStatus
    {
        Invalid,
        Created,
        Updated,
        Removed
    }

    /**
        <summary>
            Triggered when an Entity status is changed in the game's updating collection
        </summary>
    */

    [DataContract]
    [Serializable]
    public class EntityStatusChangedMessage : Message
    {
        [DataMember]
        public object Entity { get; set; }

        [DataMember]
        public EntityStatus Status { get; set; } = EntityStatus.Invalid;
    }
}
