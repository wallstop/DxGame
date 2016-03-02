using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging
{
    /**
        <summary>
            Sent to an entity to notify it that it has received Experience
        </summary>
    */

    [DataContract]
    [Serializable]
    public class ExperiencedReceivedMessage : Message
    {
        [DataMember]
        public Experience.Experience Experience { get; set; }

        public ExperiencedReceivedMessage(Experience.Experience experience)
        {
            Experience = experience;
        }
    }
}