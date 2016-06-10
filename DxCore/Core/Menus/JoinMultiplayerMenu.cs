﻿using System;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Network;
using DxCore.Core.GraphicsWidgets;
using DxCore.Core.Input;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Menus
{
    public class JoinMultiplayerMenu : Menu
    {
        protected TextBox PortBox { get; set; }
        protected TextBox AddressBox { get; set; }
        protected SpriteFont SpriteFont { get; set; }

        public override void Initialize()
        {
            SpriteFont = DxGame.Instance.Content.Load<SpriteFont>("Fonts/ComicSans");

            var portBoxSpatial =
                (SpatialComponent)
                    SpatialComponent.Builder()
                        .WithDimensions(new DxVector2
                        {
                            X = 200.0f,
                            Y = SpriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                        }).WithPosition(new DxVector2(400, 300)).Build();

            var addressBoxSpatial =
                (SpatialComponent)
                    SpatialComponent.Builder()
                        .WithDimensions(new DxVector2
                        {
                            X = 200.0f,
                            Y = SpriteFont.LineSpacing + 2 /* wiggle room for cursor */ // TODO: Fix this
                        }).WithPosition(new DxVector2(400, 400)).Build();

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            PortBox =
                TextBox.Builder()
                    .WithSpatialComponent(portBoxSpatial)
                    .WithBackgroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5)
                    .WithValidKeys(KeyboardEvent.NumericKeys)
                    .WithSpriteFont(SpriteFont)
                    .Build();

            const string portString = "Port:";
            var portLabel =
                new MenuItem().WithSpriteFont(SpriteFont)
                    .WithText(portString)
                    .WithSpace(new DxRectangle
                    {
                        X = portBoxSpatial.Space.X - /* Pixel Width of "Port:" */ SpriteFont.MeasureString(portString).X,
                        Y = portBoxSpatial.Space.Y,
                        Width = SpriteFont.MeasureString(portString).X,
                        Height = SpriteFont.MeasureString(portString).Y
                    });

            AddressBox =
                TextBox.Builder()
                    .WithSpatialComponent(addressBoxSpatial)
                    .WithBackgroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(100)
                    .WithSpriteFont(SpriteFont)
                    .Build();

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
            var connectButton =
                new MenuItem().WithSpriteFont(SpriteFont)
                    .WithText(connectString)
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

            EntityCreatedMessage addressBoxCreated = new EntityCreatedMessage(AddressBox);
            addressBoxCreated.Emit();

            EntityCreatedMessage portBoxCreated = new EntityCreatedMessage(PortBox);
            portBoxCreated.Emit();
        }

        public override void Remove()
        {
            PortBox.Remove();
            AddressBox.Remove();
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
                ((NetworkClient) new NetworkClient().WithConfiguration(config)).WithNetworkClientConfig(
                    networkClientConfig);
            client.Initialize();
            var networkModel = DxGame.Instance.Model<NetworkModel>();
            networkModel.AttachClient(client);
            client.EstablishConnection();

            var game = DxGame.Instance;
            Remove();
            game.UpdateMode = UpdateMode.Cooperative;
            game.AttachModel(new GameModel());
        }
    }
}