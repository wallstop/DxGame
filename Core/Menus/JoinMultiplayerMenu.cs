using System;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Network;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class JoinMultiplayerMenu : Menu
    {
        protected TextBox PortBox { get; set; }
        protected TextBox AddressBox { get; set; }

        public JoinMultiplayerMenu(DxGame game)
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

            var addressBoxSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2
                {
                    X = 200.0f,
                    Y = spriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                }).WithPosition(500, 500);

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            PortBox =
                new TextBox(DxGame).WithSpatialComponent(portBoxSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5)
                    // Only allow numeric values for ports
                    .WithValidKeys(KeyboardEvent.NumericKeys)
                    .WithSpriteFont(spriteFont);

            const string portString = "Port:";
            var portLabel =
                new MenuItem().WithSpriteFont(spriteFont)
                    .WithText(portString)
                    .WithSpace(new Rectangle2f
                    {
                        X = portBoxSpatial.Space.X - /* Pixel Width of "Port:" */ spriteFont.MeasureString(portString).X,
                        Y = portBoxSpatial.Space.Y,
                        Width = portBoxSpatial.Width,
                        Height = portBoxSpatial.Height
                    });

            AddressBox =
                new TextBox(DxGame).WithSpatialComponent(addressBoxSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(100)
                    .WithSpriteFont(spriteFont);

            const string addressString = "Address:";
            var addressLabel =
                new MenuItem().WithSpriteFont(spriteFont)
                    .WithText(addressString)
                    .WithSpace(new Rectangle2f
                    {
                        X =
                            addressBoxSpatial.Space.X - /* Pixel Width of "Address:" */
                            spriteFont.MeasureString(addressString).X,
                        Y = addressBoxSpatial.Space.Y,
                        Width = addressBoxSpatial.Width,
                        Height = addressBoxSpatial.Height
                    });

            var connectButton = new MenuItem().WithSpriteFont(spriteFont).WithText("Connect")
                .WithSpace(new Rectangle2f
                {
                    X = portLabel.Space.X,
                    Y = portLabel.Space.Y + 100,
                    Width = portLabel.Space.Width,
                    Height = portLabel.Space.Height
                })
                .WithAction(ConnectAction);

            MenuItems.Add(portLabel);
            MenuItems.Add(addressLabel);
            MenuItems.Add(connectButton);
        }

        public override void Remove()
        {
            DxGame.RemoveComponent(PortBox);
            DxGame.RemoveComponent(AddressBox);
            base.Remove();
        }

        protected void ConnectAction()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("DxGame");
            MultiplayerReceiveMenu clientMenu =
                new MultiplayerReceiveMenu(DxGame).WithNetConfig(config)
                    .WithIpAddress(AddressBox.Text)
                    .WithPort(Convert.ToInt32(PortBox.Text));

            Remove();
            DxGame.AddAndInitializeComponent(clientMenu);
            var networkClient = new TestNetworkClient(DxGame).WithMultiplayerReceiveMenu(clientMenu);
            DxGame.AddAndInitializeComponent(networkClient);
        }
    }
}