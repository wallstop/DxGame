using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.Components.Network
{
    public class NetworkServer : NetworkComponent
    {
        public NetworkServer(DxGame game) : base(game)
        {
            UpdatePriority = UpdatePriority.NETWORK_RECEIVE;
        }
    }
}
