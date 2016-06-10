using System;
using System.Runtime.Serialization;
using DXGame.Core;

namespace DxCore.Core.Messaging
{
    /**
        <summary>
            Sent to an entity to notify it that it has received Experience
        </summary>
    */

    [DataContract]
    [Serializable]
    public class ExperiencedReceivedMessage : Message, ITargetedMessage
    {
        [DataMember]
        public Experience.Experience Experience { get; set; }

        public ExperiencedReceivedMessage(Experience.Experience experience)
        {
            Experience = experience;
        }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}