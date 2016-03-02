using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    /**
        TODO: Come up with a better, simpler, unified lerp interface & lerp network message handling
    */
    [DataContract]
    [Serializable]
    class DxVectorLerpMessage : NetworkMessage
    {
        [DataMember]
        public UniqueId EntityId { get; set; }

        [DataMember]
        public DxVector2 CurrentLerpValue { get; set; }

        public DxVectorLerpMessage()
        {
            
        }

        public DxVectorLerpMessage(UniqueId entityId, DxVector2 currentLerpValue)
        {
            Validate.IsNotNull(entityId);
            EntityId = entityId;
            CurrentLerpValue = currentLerpValue;
        }
    }
}
