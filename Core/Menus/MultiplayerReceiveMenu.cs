using System;
using System.Diagnostics;
using DXGame.Core.Frames;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MultiplayerReceiveMenu : Menu
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MultiplayerReceiveMenu));

        public NetPeerConfiguration NetConfig { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        protected MenuItem NetworkText { get; set; }
        protected SpriteFont SpriteFont { get; set; }

        public MultiplayerReceiveMenu(DxGame game) : base(game)
        {
        }

        public MultiplayerReceiveMenu WithNetConfig(NetPeerConfiguration config)
        {
            // Don't need to check null or default right now, as config will most likely be default (empty)
            NetConfig = config;
            return this;
        }

        public MultiplayerReceiveMenu WithSpriteFont(SpriteFont spriteFont)
        {
            GenericUtils.CheckNullOrDefault(spriteFont,
                "Cannot createa a MultiplayerReceiveMenu with a null/default SpriteFont");
            SpriteFont = spriteFont;
            return this;
        }

        public MultiplayerReceiveMenu WithIpAddress(string ipAddress)
        {
            GenericUtils.CheckNullOrDefault(ipAddress, "Cannot have a null/default string as an IP Address!");
            IpAddress = ipAddress;
            return this;
        }

        public MultiplayerReceiveMenu WithPort(int port)
        {
            Debug.Assert(port >= 0, "Cannot have negative ports!");
            Port = port;
            return this;
        }

        public override void Initialize()
        {
            NetworkText =
                new MenuItem().WithSpriteFont(SpriteFont)
                    .WithText("")
                    .WithSpace(new Rectangle2f
                    {
                        X = 200,
                        Y = 200,
                        Width = 200,
                        Height = 200
                    });

            MenuItems.Add(NetworkText);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            FrameModel frameModel = DxGame.Model<FrameModel>();
            GameTimeFrame latestFrame = frameModel.LatestFrame();
            if (!GenericUtils.IsNullOrDefault(latestFrame))
            {
                var testString = latestFrame.TestString;
                LOG.Info(String.Format("Received message: {0}", testString));
                NetworkText.Text = testString;
            }
            else
            {
                NetworkText.Text = "";
            }
            base.Update(gameTime);
        }
    }
}