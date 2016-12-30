using System;
using System.Runtime.Serialization;
using DxCore.Core.Physics;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging.Physics
{
    [Serializable]
    [DataContract]
    public class PhysicsAttachment : Message, ITargetedMessage
    {
        [DataMember]
        private object PhysicsInteraction { get; set; }

        private PhysicsAttachment(object interaction, UniqueId target)
        {
            Validate.Hard.IsNotNullOrDefault(interaction);
            PhysicsInteraction = interaction;
            Validate.Hard.IsNotNullOrDefault(target);
            Target = target;
        }

        public PhysicsAttachment(Force force, UniqueId target) : this((object) force, target) {}

        public PhysicsAttachment(Impulse impulse, UniqueId target) : this((object) impulse, target) {}

        public PhysicsAttachment(Nullification nullification, UniqueId target) : this((object) nullification, target) {}

        [DataMember]
        public UniqueId Target { get; private set; }

        public Type Extract(out object interactionObject)
        {
            interactionObject = PhysicsInteraction;
            return PhysicsInteraction.GetType();
        }
    }
}