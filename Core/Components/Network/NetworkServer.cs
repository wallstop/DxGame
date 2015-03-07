﻿using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Network
{
    public class NetworkServer : NetworkComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkServer));

        public NetServer ServerConnection
        {
            get { return Connection as NetServer; }
        }

        public Dictionary<NetConnection, FrameModel> ClientFrameStates { get; set; }

        public NetworkServer(DxGame game)
            : base(game)
        {
            ClientFrameStates = new Dictionary<NetConnection, FrameModel>();
        }

        public override NetworkComponent WithConfiguration(NetPeerConfiguration configuration)
        {
            // Make sure we enable Approvals so we can accept Client Connections
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Connection = new NetServer(configuration);
            return this;
        }

        public override void EstablishConnection()
        {
            ServerConnection.Start();
        }

        public override void RouteDataOnMessageType(NetIncomingMessage incomingMessage, GameTime gameTime)
        {
            // TODO: Deal with gameTime
            switch (incomingMessage.MessageType)
            {
                case NetIncomingMessageType.Error:
                    ProcessError(incomingMessage);
                    break;
                case NetIncomingMessageType.Data:
                    ProcessData(incomingMessage);
                    break;
                case NetIncomingMessageType.ConnectionLatencyUpdated:
                    ProcessConnectionLatencyUpdated(incomingMessage);
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    ProcessConnectionApproval(incomingMessage);
                    break;
                default:
                    ProcessUnhandledMessageType(incomingMessage);
                    break;
            }
        }

        public override void SendData(GameTime gameTime)
        {
            foreach (NetConnection connection in ClientFrameStates.Keys)
            {
                // Quick and dirty for now - do some nice differentials later
                var message = new GameStateKeyFrame {Components = DxGame.Components.ToList(), GameTime = gameTime, MessageType = MessageType.SERVER_DATA_KEYFRAME};
                var outgoingMessage = message.ToNetOutgoingMessage(connection.Peer);
                connection.SendMessage(outgoingMessage, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkServer");
            ServerConnection.Shutdown("NetworkServer shutting down calmly");
        }

        protected void ProcessUnhandledMessageType(NetIncomingMessage message)
        {
            GenericUtils.HardFail(LOG,
                String.Format("NetworkServer currently doesn't support messages of the type {0}. Message: {1}",
                    message.MessageType, message));
        }

        protected void ProcessError(NetIncomingMessage message)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received IncomingMessage with error type, this shouldn't happen! Message: {0}", message));
        }

        protected void ProcessConnectionLatencyUpdated(NetIncomingMessage message)
        {
            // TODO: Handle latency updates (currently not supported)
            LOG.Info(String.Format("Received LatencyUpdate {0}", message));
            throw new NotImplementedException();
        }

        protected void ProcessConnectionApproval(NetIncomingMessage message)
        {
            if (ClientFrameStates.ContainsKey(message.SenderConnection))
            {
                // TODO: Metrics
                LOG.Error(String.Format("Denying Connection {0}, this connection is currently already approved.",
                    message.SenderConnection));
                message.SenderConnection.Deny();
            }

            /*
                Only approve the connection here - wait to instantiate the client's FrameModel until they send our 
                own special ClientApproval packet 
            */
            LOG.Info(String.Format("Approving NetworkServer client connection {0}", message.SenderConnection));
            message.SenderConnection.Approve();
        }

        protected void ProcessData(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot process server data on a null message!");
            var networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            GenericUtils.CheckNull(networkMessage,
                "Could not properly format a NetworkMessage from the NetIncomingMessage");
            switch (networkMessage.MessageType)
            {
            case MessageType.CLIENT_CONNECTION_REQUEST:
                HandleClientConnectionRequest(networkMessage);
                break;
            case MessageType.CLIENT_DATA_DIFF:
                HandleClientDataDiff(networkMessage);
                break;
            case MessageType.CLIENT_KEY_FRAME:
                HandleClientDataKeyFrame(networkMessage);
                break;
            case MessageType.SERVER_DATA_DIFF:
                HandleServerDataDiff(networkMessage);
                break;
            case MessageType.SERVER_DATA_KEYFRAME:
                HandleServerDataKeyframe(networkMessage);
                break;
            }
        }

        protected void HandleClientDataDiff(NetworkMessage message)
        {
            var clientDataDiffMessage = ConvertMessageType<GameStateDiff>(message);

            // TODO: Nice lerp logic. For now, just ignore client messages

            //var clientConnection = clientDataDiffMessage.Connection;
        }

        protected void HandleClientDataKeyFrame(NetworkMessage message)
        {
            // TODO: Nice lerp logic. For now, just ignore client messages
        }

        protected void HandleClientConnectionRequest(NetworkMessage message)
        {
            var clientConnectionRequest = ConvertMessageType<ClientConnectionRequest>(message);

            var clientConnection = clientConnectionRequest.Connection;
            if (ClientFrameStates.ContainsKey(clientConnection))
            {
                GenericUtils.HardFail(LOG,
                    String.Format(
                        "Received ClientConnectionRequest that we're already tracking, this is an issue! Request: {0}",
                        clientConnectionRequest));
                return;
            }

            var frameModel = new FrameModel(DxGame);
            ClientFrameStates.Add(clientConnection, frameModel);
            LOG.Info(String.Format("Successfully initialized ClientConnection {0} for player {1}", clientConnection,
                clientConnectionRequest.PlayerName));
        }

        protected void HandleServerDataDiff(NetworkMessage message)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received a Server Data Diff {0} from a client. This should not happen",
                    message));
        }

        protected void HandleServerDataKeyframe(NetworkMessage message)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received a Server Data Keyframe {0} from a client. This should not happen",
                    message));
        }
    }
}