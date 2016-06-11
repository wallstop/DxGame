using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

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
            Validate.IsNotNull(playerId, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(playerId)));
            PlayerId = playerId;
        }
    }
}
