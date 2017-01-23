using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using WallNetCore.Validate;

namespace DxCore.Core.Network
{
    /**
        TODO: Come up with a better, simpler, unified lerp interface & lerp network message handling
    */

    [DataContract]
    [Serializable]
    internal class DxVectorLerpMessage : NetworkMessage
    {
        [DataMember]
        public DxVector2 CurrentLerpValue { get; set; }

        [DataMember]
        public UniqueId EntityId { get; set; }

        public DxVectorLerpMessage() {}

        public DxVectorLerpMessage(UniqueId entityId, DxVector2 currentLerpValue)
        {
            Validate.Hard.IsNotNull(entityId);
            EntityId = entityId;
            CurrentLerpValue = currentLerpValue;
        }
    }
}