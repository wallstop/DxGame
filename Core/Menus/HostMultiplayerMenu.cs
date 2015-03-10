using System;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Network;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class HostMultiplayerMenu : Menu
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (HostMultiplayerMenu));
        protected TextBox PortBox { get; set; }

        public HostMultiplayerMenu(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/ComicSans");

            var portBoxSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2
                {
                    X = 200.0f,
                    Y = spriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                }).WithPosition(600, 500);

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            PortBox =
                new TextBox(DxGame).WithSpatialComponent(portBoxSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5)
                    // Only allow numeric values for ports
                    .WithValidKeys(KeyboardEvent.NumericKeys)
                    .WithSpriteFont(spriteFont);

            var portLabel =
                new MenuItem().WithSpriteFont(spriteFont)
                    .WithText("Port:")
                    .WithSpace(new Rectangle2f
                    {
                        X = portBoxSpatial.Space.X - /* Pixel Width of "Port:" */ spriteFont.MeasureString("Port:").X,
                        Y = portBoxSpatial.Space.Y,
                        Width = portBoxSpatial.Width,
                        Height = portBoxSpatial.Height
                    });


            DxGame.AddAndInitializeComponent(PortBox);

            var hostLabel = new MenuItem().WithSpriteFont(spriteFont)
                .WithText("Host")
                .WithSpace(new Rectangle2f
                {
                    X = portLabel.Space.X,
                    Y = portLabel.Space.Y + 100,
                    Width = portLabel.Space.Width,
                    Height = portLabel.Space.Height
                })
                .WithAction(HostAction);

            MenuItems.Add(portLabel);
            MenuItems.Add(hostLabel);

            base.Initialize();
        }

        public override void Remove()
        {
            DxGame.RemoveComponent(PortBox);
            base.Remove();
        }

        public override void SerializeTo(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        public override void DeserializeFrom(NetIncomingMessage messsage)
        {
            throw new NotImplementedException();
        }

        private void HostAction()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("DxGame");
            int port = 0;
            try
            {
                port = Convert.ToInt32(PortBox.Text);
            }
            catch (Exception e)
            {
                LOG.Error("Could not format port", e);
            }

            config.Port = port;
            // TODO: Change this to 4 / whatever our max player limit is
            config.MaximumConnections = 1;

            NetworkServer server = (NetworkServer) new NetworkServer(DxGame).WithConfiguration(config);
            var networkModel = DxGame.Model<NetworkModel>();
            networkModel.AttachServer(server);
            server.EstablishConnection();

            Remove();
            DxGame.AddAndInitializeComponent(new GameModel(DxGame));
        }
    }
}