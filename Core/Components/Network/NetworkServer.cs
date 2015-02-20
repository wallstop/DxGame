using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Components.Network
{
    public class NetworkServer : NetworkComponent
    {
        public NetServer ServerConnection
        {
            get { return Connection as NetServer; }
        }

        public NetworkServer(DxGame game) 
            : base(game)
        {
        }

        protected override void EstablishConnection()
        {
            throw new NotImplementedException();
        }

        public override void ReceiveData()
        {
            throw new NotImplementedException();
        }

        public override void SendData()
        {
            throw new NotImplementedException();
        }
    }
}
