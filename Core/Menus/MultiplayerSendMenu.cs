using DXGame.Core.Components.Advanced;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/ComicSans");

            var sendTextSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2
                {
                    X = 200.0f,
                    Y = spriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                }).WithPosition(600, 500);

            SendText =
                new TextBox(DxGame).WithSpatialComponent(sendTextSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithSpriteFont(spriteFont);

            DxGame.AddAndInitializeComponent(SendText);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}