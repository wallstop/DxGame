using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Network
{
    /**
        <summary>
            Configuration for network clients. Anything client-specific can be stored up here (PlayerName, favorite color, etc) for use with a NetworkClient.

            In particular, the IpAddress and Port that the client will be connecting to (NOT the Ip and Port of the client, mind)
        </summary>
    */

    [DataContract]
    [Serializable]
    public struct NetworkClientConfig
    {
        // TODO: Abstract out to own validation class?
        [DataMember]
        public string IpAddress { get; }

        [DataMember]
        public Port Port { get; }

        [DataMember]
        public Dictionary<string, object> Metadata { get; }

        public NetworkClientConfig(string ipAddress, Port port)
        {
            Validate.Hard.IsNotNullOrDefault(ipAddress,
                StringUtils.GetFormattedNullOrDefaultMessage(typeof(NetworkClientConfig), nameof(ipAddress)));
            IpAddress = ipAddress;
            Validate.Hard.IsNotNullOrDefault(port,
                StringUtils.GetFormattedNullOrDefaultMessage(typeof(NetworkClientConfig), port));
            Port = port;
            Metadata = new Dictionary<string, object>();
        }
    }
}
