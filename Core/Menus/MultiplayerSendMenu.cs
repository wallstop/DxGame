using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;

namespace DXGame.Core.Menus
{
    public class MultiplayerSendMenu : Menu
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MultiplayerSendMenu));

        protected NetPeerConfiguration NetConfig { get; set; }

        public MultiplayerSendMenu(DxGame game) : base(game)
        {
        }

        public MultiplayerSendMenu WithNetConfig(NetPeerConfiguration config)
        {
            GenericUtils.CheckNullOrDefault(config, "Cannot create a MultiplayerSendMenu with a null/default config");
            NetConfig = config;
            return this;
        }
    }
}
