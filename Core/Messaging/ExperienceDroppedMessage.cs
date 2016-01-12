using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging
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
        public Optional<Team> SourceTeam { get; }
        public DxVector2 Position { get; }
        public float Radius { get; }
        public Experience.Experience Experience { get; }

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