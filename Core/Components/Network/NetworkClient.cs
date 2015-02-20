﻿using DXGame.Core.Components.Basic;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    public class NetworkClient : NetworkComponent
    {
        public NetClient ClientConnection
        {
            get { return Connection as NetClient; }
        }

        public NetworkClient(DxGame game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void EstablishConnection()
        {
            throw new System.NotImplementedException();
        }

        public override void ReceiveData()
        {
            throw new System.NotImplementedException();
        }

        public override void SendData()
        {
            throw new System.NotImplementedException();
        }
    }
}