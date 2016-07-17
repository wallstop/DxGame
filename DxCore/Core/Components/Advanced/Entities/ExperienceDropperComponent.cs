using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Team;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Entities
{
    [DataContract]
    [Serializable]
    public class ExperienceDropperComponent : Component
    {
        public static readonly float DEFAULT_RADIUS = 250;

        [DataMember]
        public float Radius { get; }

        [DataMember]
        public Experience.Experience Experience { get; }

        public ExperienceDropperComponent(Experience.Experience experience) : this(DEFAULT_RADIUS, experience) {}

        public ExperienceDropperComponent(float radius, Experience.Experience experience)
        {
            Validate.Hard.IsTrue(radius > 0,
                $"Cannot create a {typeof(ExperienceDropperComponent)} with a radius of {radius}");
            Validate.Hard.IsNotNullOrDefault(experience, this.GetFormattedNullOrDefaultMessage(experience));
            Radius = radius;
            Experience = experience;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<EntityDeathMessage>(HandleEntityDeath);
            base.OnAttach();
        }

        protected virtual void HandleEntityDeath(EntityDeathMessage deathMessage)
        {
            DxCore.Core.Team sourceTeam = Parent?.ComponentOfType<TeamComponent>()?.Team;
            DxVector2 sourcePosition = Parent?.ComponentOfType<PhysicsComponent>()?.Center ?? new DxVector2();
            ExperienceDroppedMessage experienceDropped = new ExperienceDroppedMessage(sourceTeam, sourcePosition, Radius,
                Experience);
            experienceDropped.Emit();
        }
    }
}