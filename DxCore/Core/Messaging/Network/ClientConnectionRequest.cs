using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging.Network
{
    [Serializable]
    [DataContract]
    public class ClientConnectionRequest : NetworkMessage
    {
        [DataMember]
        public Dictionary<string, object> Metadata { get; }

        public ClientConnectionRequest() : this(new Dictionary<string, object>()) {}

        public ClientConnectionRequest(Dictionary<string, object> metadata)
        {
            Validate.Hard.IsNotNull(metadata, this.GetFormattedNullOrDefaultMessage(nameof(metadata)));
            Metadata = metadata;
        }
    }
}