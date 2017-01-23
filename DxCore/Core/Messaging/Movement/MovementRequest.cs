using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils.Distance;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging.Movement
{
    [Serializable]
    [DataContract]
    public class MovementRequest : Message, ITargetedMessage
    {
        [DataMember]
        public Direction? Direction { get; private set; }

        public MovementRequest(Direction? direction, UniqueId target)
        {
            Validate.Hard.IsNotNull(target);
            Target = target;
            Direction = direction;
        }

        [DataMember]
        public UniqueId Target { get; private set; }
    }
}