using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Network;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Network
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
