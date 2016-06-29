using System;
using System.Runtime.Serialization;
using DxCore.Core.Physics;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    /**
        <summary>
            Alerts an entity that they're about to have a big ol' force attached to them. Might move them somewhere.

            WHO KNOWS

            Relevant components should pick up the provided force and attach it to themselves.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class AttachForce : Message, ITargetedMessage
    {
        [DataMember]
        public UniqueId Target { get; private set; }

        [DataMember]
        public Force Force { get; private set; }

        public AttachForce(UniqueId target, Force force)
        {
            Validate.Hard.IsNotNullOrDefault(target, () => this.GetFormattedNullOrDefaultMessage(nameof(target)));
            Target = target;
            Validate.Hard.IsNotNullOrDefault(force, () => this.GetFormattedNullOrDefaultMessage(force));
            Force = force;
        }
    }
}
