using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace DXGame.Core.Network
{
    public class ClientConnectionRequest : NetworkMessage
    {

        public String PlayerName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ClientConnectionRequest(NetIncomingMessage incomingMessage) 
            : base(incomingMessage)
        {
        }




    }
}
