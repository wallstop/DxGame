using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    /**
        <summary>
            Notifies entities that "Experience" has been dropped
        </summary>
    */

    [DataContract]
    [Serializable]
    public class ExperienceDroppedMessage : Message
    {
        [DataMember]
        private Team SourceTeam { get; set; }

        [DataMember]
        public DxVector2 Position { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [DataMember]
        public Experience.Experience Experience { get; set; }

        public ExperienceDroppedMessage(Team sourceTeam, DxVector2 position, float radius,
            Experience.Experience experience)
        {
            Validate.Hard.IsPositive(radius,
                () => $"Canot create a {nameof(ExperienceDroppedMessage)} with a {nameof(radius)} of {radius}");
            Validate.Hard.IsNotNullOrDefault(experience, () => this.GetFormattedNullOrDefaultMessage(experience));
            SourceTeam = sourceTeam;
            Position = position;
            Radius = radius;
            Experience = experience;
        }

        public bool GetSourceTeam(out Team sourceTeam)
        {
            if(ReferenceEquals(SourceTeam, null))
            {
                sourceTeam = null;
                return false;
            }

            sourceTeam = SourceTeam;
            return true;
        }
    }
}