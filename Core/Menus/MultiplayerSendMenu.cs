using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;

namespace DXGame.Core.Menus
{
    // TODO: Remove, this is all test code
    public class MultiplayerSendMenu : Menu
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MultiplayerSendMenu));

        public NetPeerConfiguration NetConfig { get; set; }
        public TextBox SendText { get; set; }

        public MultiplayerSendMenu(DxGame game) : base(game)
        {
        }

        public MultiplayerSendMenu WithNetConfig(NetPeerConfiguration config)
        {
            GenericUtils.CheckNullOrDefault(config, "Cannot create a MultiplayerSendMenu with a null/default config");
            NetConfig = config;
            return this;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}