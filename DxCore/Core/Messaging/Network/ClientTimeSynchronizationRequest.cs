using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DXGame.Core.Primitives;
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
            Validate.IsNotNullOrDefault(currentGameTime,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(currentGameTime)));
            ClientSideGameTime = currentGameTime.TotalGameTime;
        }
    }
}
