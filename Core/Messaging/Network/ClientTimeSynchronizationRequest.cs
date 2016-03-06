using System;
using System.Runtime.Serialization;
using DXGame.Core.Network;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Network
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
