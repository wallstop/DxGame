﻿using DXGame.Core.Components.Basic;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Components.Advanced
{
    public class PropertiesComponent : Component
    {
        public PropertiesComponent(DxGame game)
            : base(game)
        {
        }

        public override void Write(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void Read(NetIncomingMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}