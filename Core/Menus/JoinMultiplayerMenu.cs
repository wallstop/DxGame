﻿using System;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Network;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Wrappers;
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
        protected SpriteFont SpriteFont { get; set; }

        public JoinMultiplayerMenu(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            SpriteFont = DxGame.Content.Load<SpriteFont>("Fonts/ComicSans");

            var portBoxSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new DxVector2
                {
                    X = 200.0f,
                    Y = SpriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                }).WithPosition(400, 300);

            var addressBoxSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new DxVector2
                {
                    X = 200.0f,
                    Y = SpriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                }).WithPosition(400, 400);

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            PortBox =
                new TextBox(DxGame).WithSpatialComponent(portBoxSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5)
                    // Only allow numeric values for ports
                    .WithValidKeys(KeyboardEvent.NumericKeys)
                    .WithSpriteFont(SpriteFont);

            const string portString = "Port:";
            var portLabel =
                new MenuItem().WithSpriteFont(SpriteFont)
                    .WithText(portString)
                    .WithSpace(new DxRectangle
                    {
                        X =
                            portBoxSpatial.Space.X
                            - /* Pixel Width of "Port:" */ SpriteFont.MeasureString(portString).X,
                        Y = portBoxSpatial.Space.Y,
                        Width = SpriteFont.MeasureString(portString).X,
                        Height = SpriteFont.MeasureString(portString).Y
                    });

            AddressBox =
                new TextBox(DxGame).WithSpatialComponent(addressBoxSpatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(100)
                    .WithSpriteFont(SpriteFont);

            const string addressString = "Address:";
            var addressLabel =
                new MenuItem().WithSpriteFont(SpriteFont)
                    .WithText(addressString)
                    .WithSpace(new DxRectangle
                    {
                        X =
                            addressBoxSpatial.Space.X - /* Pixel Width of "Address:" */
                            SpriteFont.MeasureString(addressString).X,
                        Y = addressBoxSpatial.Space.Y,
                        Width = SpriteFont.MeasureString(addressString).X,
                        Height = SpriteFont.MeasureString(addressString).Y
                    });

            const string connectString = "Connect";
            var connectButton = new MenuItem().WithSpriteFont(SpriteFont).WithText(connectString)
                .WithSpace(new DxRectangle
                {
                    X = addressLabel.Space.X,
                    Y = addressLabel.Space.Y + 100,
                    Width = SpriteFont.MeasureString(connectString).X,
                    Height = SpriteFont.MeasureString(connectString).Y
                })
                .WithAction(ConnectAction);

            MenuItems.Add(portLabel);
            MenuItems.Add(addressLabel);
            MenuItems.Add(connectButton);
            DxGame.AddAndInitializeComponent(AddressBox);
            DxGame.AddAndInitializeComponent(PortBox);
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
            var networkClientConfig = new NetworkClientConfig
            {
                IpAddress = AddressBox.Text,
                Port = Convert.ToInt32(PortBox.Text),
                PlayerName = "DickButt"
            };

            var client =
                ((NetworkClient) new NetworkClient(DxGame).WithConfiguration(config))
                    .WithNetworkClientConfig(
                        networkClientConfig);
            var networkModel = DxGame.Model<NetworkModel>();
            networkModel.AttachClient(client);
            client.EstablishConnection();

            Remove();
            DxGame.AttachModel(new GameModel(DxGame));
        }
    }
}