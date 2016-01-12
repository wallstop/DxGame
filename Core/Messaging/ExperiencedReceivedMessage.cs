using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

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
        public Experience.Experience Experience { get; }

        public ExperiencedReceivedMessage(Experience.Experience experience)
        {
            Validate.IsNotNullOrDefault(experience, StringUtils.GetFormattedNullOrDefaultMessage(this, experience));
            Experience = experience;
        }
    }
}