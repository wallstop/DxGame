using System;
using DXGame.Core.Menus;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    // TODO: Remove
    public class TestNetworkServer : NetworkServer
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (TestNetworkServer));
        protected MultiplayerSendMenu Menu { get; set; }

        public TestNetworkServer(DxGame game)
            : base(game)
        {
        }

        public TestNetworkServer WithMultiplayerMenu(MultiplayerSendMenu sendMenu)
        {
            GenericUtils.CheckNullOrDefault(sendMenu,
                "Cannot create a server based off of a null/default Multiplayer Send Menu");
            Menu = sendMenu;
            return this;
        }

        public override void Update(GameTime gameTime)
        {
            GenericUtils.CheckNullOrDefault(Menu, "Cannot enter Server Update Loop without a valid Menu to grab data from");

            if (GenericUtils.IsNullOrDefault(Connection))
            {
                var config = Menu.NetConfig;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                LOG.Info(String.Format("Attempting to create a network configuration with config {0}", config));
                Connection = new NetServer(config);
                Connection.Start();
            }

            NetIncomingMessage inMessage = Connection.ReadMessage();
            /*
                TODO : Make this its own thread, or thread-like thing. We need to pull ALL messages received, not just the current one
            */
            if (inMessage != null)
            {
                switch (inMessage.MessageType)
                {
                case NetIncomingMessageType.ConnectionApproval:
                    HandleConnectionApproval(inMessage);
                    break;
                case NetIncomingMessageType.Data:
                    HandleData(inMessage);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    HandleStatusChanged(inMessage);
                    break;
                }

                NetOutgoingMessage outMessage = Connection.CreateMessage();
                outMessage.Write(Menu.SendText.Text);
                LOG.Info(String.Format("Sending message: {0}, outMessage"));
                Connection.SendMessage(outMessage, inMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            }

            base.Update(gameTime);
        }

        private void HandleConnectionApproval(NetIncomingMessage message)
        {
            // TODO: Come up with common serialize / deserialize format.
            if (message.ReadByte() == (byte) PacketTypes.LOGIN)
            {
                LOG.Info(String.Format("Received LOGIN message"));
                message.SenderConnection.Approve();
            }
        }

        private void HandleData(NetIncomingMessage message)
        {
            LOG.Info(String.Format("Received DATA message"));
            // Right now we don't care if the clients send us anything
        }

        private void HandleStatusChanged(NetIncomingMessage message)
        {
            LOG.Info(String.Format("Received STATUS_CHANGED message"));
            // Also don't care if the clients change their status.
        }
    }
}