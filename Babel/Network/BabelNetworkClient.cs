using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Network;
using Lidgren.Network;

namespace Babel.Network
{
    public class BabelNetworkClient : AbstractNetworkClient
    {
        public BabelNetworkClient(NetworkClientConfig clientConfig, NetPeerConfiguration netPeerConfig) : base(netPeerConfig, clientConfig) {}
    }
}
