using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using WallNetCore.Validate;

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
                () => this.GetFormattedNullOrDefaultMessage(nameof(currentGameTime)));
            ClientSideGameTime = currentGameTime.TotalGameTime;
        }
    }
}