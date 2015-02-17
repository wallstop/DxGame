using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DXGame.Core.Frames;
using DXGame.Core.Menus;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    // TODO: Remove
    public class TestNetworkClient : NetworkClient
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(TestNetworkClient));

        protected MultiplayerReceiveMenu Menu { get; set; }

        protected bool Connected { get; set; }

        public TestNetworkClient(DxGame game)
            : base(game)
        {
            Connected = false;
        }

        public TestNetworkClient WithMultiplayerReceiveMenu(MultiplayerReceiveMenu receiveMenu)
        {
            GenericUtils.CheckNullOrDefault(receiveMenu,
                "Cannot create a client based off of a null/default Multiplayer Receive Menu");
            Menu = receiveMenu;
            return this;
        }

        public override void Update(GameTime gameTime)
        {
            GenericUtils.CheckNullOrDefault(Menu, "Cannot enter Client Update Loop without a valid Menu to push data to");

            if (GenericUtils.IsNullOrDefault(Connection))
            {
                var config = Menu.NetConfig;
                LOG.Info(String.Format("Attempting to create a network configuration with config {0}", config));
                Connection = new NetClient(config);
                Connection.Start();
            }

            if (!Connected)
            {
                EstablishConnection();
            }
            else
            {
                ReceiveData(gameTime);
            }

            base.Update(gameTime);
        }

        private void EstablishConnection()
        {
            LOG.Info(String.Format("Attempting connection to {0} : {1}", Menu.IpAddress, Menu.Port));
            NetOutgoingMessage outMessage = Connection.CreateMessage();
            outMessage.Write((byte)PacketTypes.LOGIN);
            Connection.Connect(Menu.IpAddress, Menu.Port, outMessage);
            Connected = true;
            LOG.Info(String.Format("Established connection to {0} : {1}", Menu.IpAddress, Menu.Port));
        }

        private void ReceiveData(GameTime gameTime)
        {
            NetIncomingMessage message = Connection.ReadMessage();
            if (message != null)
            {
                string receivedText = message.ReadString();
                LOG.Info(String.Format("Received message: {0}", receivedText));
                var frameModel = DxGame.Model<FrameModel>();

                GameTimeFrame frame = new GameTimeFrame();
                frame.TimeStamp = gameTime.TotalGameTime;
                frame.TestString = receivedText;
                frameModel.AttachFrame(frame);
            }
        }
    }
}
