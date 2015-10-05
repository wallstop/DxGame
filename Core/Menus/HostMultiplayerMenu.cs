using System;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Network;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Menus
{
    public class HostMultiplayerMenu : Menu
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        protected TextBox PortBox { get; set; }

        public HostMultiplayerMenu(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/ComicSans");

            var portBoxSpatial = (SpatialComponent) SpatialComponent.Builder().WithDimensions(new DxVector2
            {
                X = 200.0f,
                Y = spriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
            }).WithPosition(new DxVector2(600, 500)).Build();

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            PortBox =
                TextBox.Builder()
                    .WithSpatialComponent(portBoxSpatial)
                    .WithBackgroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5)
                    .WithValidKeys(KeyboardEvent.NumericKeys)
                    .WithSpriteFont(spriteFont)
                    .Build();

            var portLabel =
                new MenuItem().WithSpriteFont(spriteFont)
                    .WithText("Port:")
                    .WithSpace(new DxRectangle
                    {
                        X =
                            portBoxSpatial.Space.X
                            - /* Pixel Width of "Port:" */ spriteFont.MeasureString("Port:").X,
                        Y = portBoxSpatial.Space.Y,
                        Width = portBoxSpatial.Width,
                        Height = portBoxSpatial.Height
                    });


            DxGame.AddAndInitializeComponent(PortBox);

            var hostLabel = new MenuItem().WithSpriteFont(spriteFont)
                .WithText("Host")
                .WithSpace(new DxRectangle
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
                LOG.Error(e, "Could not format port");
            }

            config.Port = port;
            // TODO: Change this to 4 / whatever our max player limit is
            config.MaximumConnections = 1;

            NetworkServer server =
                (NetworkServer) new NetworkServer(DxGame).WithConfiguration(config);
            var networkModel = DxGame.Model<NetworkModel>();
            networkModel.AttachServer(server);
            server.EstablishConnection();

            var game = DxGame;
            Remove();
            game.AttachModel(new GameModel(game));
        }
    }
}