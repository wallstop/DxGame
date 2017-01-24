using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class ChangeScaleRequest : Message, ITargetedMessage
    {
        public const float DefaultScale = 1.0f;

        [DataMember]
        public float Scale { get; set; }

        public ChangeScaleRequest(UniqueId target, float scale = DefaultScale)
        {
            Validate.Hard.IsNotNull(target, () => this.GetFormattedNullOrDefaultMessage(nameof(target)));
            Target = target;
            Scale = scale;
        }

        [DataMember]
        public UniqueId Target { get; private set; }
    }
}