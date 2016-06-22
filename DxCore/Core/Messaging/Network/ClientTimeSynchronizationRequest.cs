using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class ClientTimeSynchronizationRequest : NetworkMessage
    {
        [DataMember]
        public TimeSpan ClientSideGameTime { get; private set; }

        public ClientTimeSynchronizationRequest(DxGameTime currentGameTime)
        {
            Validate.Hard.IsNotNullOrDefault(currentGameTime,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(currentGameTime)));
            ClientSideGameTime = currentGameTime.TotalGameTime;
        }
    }
}
