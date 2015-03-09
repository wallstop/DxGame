using System;
using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    public struct NetworkClientConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string PlayerName { get; set; }
    }

    public class NetworkClient : NetworkComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkClient));

        protected NetworkClientConfig ClientConfig { get; set; }

        public NetClient ClientConnection
        {
            get { return Connection as NetClient; }
        }

        public NetworkClient(DxGame game)
            : base(game)
        {
        }

        public NetworkClient WithNetworkClientConfig(NetworkClientConfig config)
        {
            GenericUtils.CheckNullOrDefault(config,
                "Cannot create a NetworkClient with a null/default NetworkClientConfig");
            ClientConfig = config;
            return this;
        }

        public override NetworkComponent WithConfiguration(NetPeerConfiguration configuration)
        {
            GenericUtils.CheckNullOrDefault(configuration,
                "Cannot create a NetworkClient with a null/default NetPeerConfiguration");
            Connection = new NetClient(configuration);
            return this;
        }

        public override void EstablishConnection()
        {
            ClientConnection.Start();

            ClientConnectionRequest request = new ClientConnectionRequest {PlayerName = ClientConfig.PlayerName};
            var outMessage = request.ToNetOutgoingMessage(ClientConnection);

            ClientConnection.Connect(ClientConfig.IpAddress, ClientConfig.Port, outMessage);
        }

        public override void RouteDataOnMessageType(NetIncomingMessage message, GameTime gameTime)
        {
            // TODO: Deal with gameTime
            switch (message.MessageType)
            {
            case NetIncomingMessageType.Data:
                ProcessData(message);
                break;
            case NetIncomingMessageType.StatusChanged:
                // TODO: Handle lol
                break;
            default:
                throw new NotImplementedException(String.Format("Currently not dealing with on MessageType {0} (TODO)",
                    message.MessageType));
            }
        }

        protected void ProcessData(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot process server data on a null message!");
            var networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            GenericUtils.CheckNull(networkMessage,
                "Could not properly format a NetworkMessage from the NetIncomingMessage");
            switch (networkMessage.MessageType)
            {
            case MessageType.SERVER_DATA_KEYFRAME:
                HandleServerDataKeyFrame(networkMessage);
                break;
            default:
                throw new NotImplementedException(String.Format("Currently not dealing with on MessageType {0} (TODO)",
                    message.MessageType));
            }
        }

        protected void HandleServerDataKeyFrame(NetworkMessage message)
        {
            var serverDataKeyFrame = ConvertMessageType<GameStateKeyFrame>(message);

            IEnumerable<IGameComponent> components = serverDataKeyFrame.Components;

            DxGame.ResetComponents(components);
        }

        public override void SendData(GameTime gameTime)
        {
            // TODO:
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkClient");
            ClientConnection.Shutdown("NetworkClient shutting down calmly");
        }

        public override void Write(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Read(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}