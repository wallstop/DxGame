using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Entities
{
    [DataContract]
    [Serializable]
    [ProtoContract]
    public class ExperienceDropperComponent : Component
    {
        public static readonly float DEFAULT_RADIUS = 250;

        [DataMember]
        [ProtoMember(1)]
        public float Radius { get; }

        [DataMember]
        [ProtoMember(2)]
        public Experience.Experience Experience { get; }

        public ExperienceDropperComponent(Experience.Experience experience) : this(DEFAULT_RADIUS, experience) {}

        public ExperienceDropperComponent(float radius, Experience.Experience experience)
        {
            Validate.IsTrue(radius > 0,
                $"Cannot create a {typeof(ExperienceDropperComponent)} with a radius of {radius}");
            Validate.IsNotNullOrDefault(experience, StringUtils.GetFormattedNullOrDefaultMessage(this, experience));
            Radius = radius;
            Experience = experience;
            MessageHandler.RegisterMessageHandler<EntityDeathMessage>(HandleEntityDeath);
        }

        protected virtual void HandleEntityDeath(EntityDeathMessage deathMessage)
        {
            Team sourceTeam = Parent?.ComponentOfType<TeamComponent>()?.Team;
            DxVector2 sourcePosition = Parent?.ComponentOfType<SpatialComponent>()?.Center ?? new DxVector2();
            ExperienceDroppedMessage experienceDropped = new ExperienceDroppedMessage(sourceTeam, sourcePosition, Radius,
                Experience);
            DxGame.Instance.BroadcastMessage(experienceDropped);
        }
    }
}