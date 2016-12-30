using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class UpdateActivePlayer : NetworkMessage
    {
        [DataMember]
        public UniqueId PlayerId { get; private set; }

        public UpdateActivePlayer(UniqueId playerId)
        {
            Validate.Hard.IsNotNull(playerId, this.GetFormattedNullOrDefaultMessage(nameof(playerId)));
            PlayerId = playerId;
        }
    }
}