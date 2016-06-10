using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core;
using DXGame.Core.Utils;

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
        public Optional<Team> SourceTeam { get; set; }
        public DxVector2 Position { get; set; }
        public float Radius { get; set; }
        public Experience.Experience Experience { get; set; }

        public ExperienceDroppedMessage(Team sourceTeam, DxVector2 position, float radius,
            Experience.Experience experience)
        {
            Validate.IsTrue(radius > 0,
                $"Canot create a {nameof(ExperienceDroppedMessage)} with a {nameof(radius)} of {radius}");
            Validate.IsNotNullOrDefault(experience, StringUtils.GetFormattedNullOrDefaultMessage(this, experience));
            SourceTeam = Optional<Team>.Of(sourceTeam);
            Position = position;
            Radius = radius;
            Experience = experience;
        }
    }
}