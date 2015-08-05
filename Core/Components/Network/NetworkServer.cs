using System;
using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Lidgren.Network;
using NLog;

namespace DXGame.Core.Components.Network
{
    /*
        TODO: Lots of things. We need to handle all kinds of messages, and some other things.
    */

    public class NetworkServer : NetworkComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public NetServer ServerConnection => Connection as NetServer;
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

        public override void RouteDataOnMessageType(NetIncomingMessage incomingMessage, DxGameTime gameTime)
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
                case NetIncomingMessageType.StatusChanged:
                    // TODO: Handle
                    break;
                default:
                    // TODO: Handle
                    break;
                //ProcessUnhandledMessageType(incomingMessage);
                //break;
            }
        }

        public override void SendData(DxGameTime gameTime)
        {
            foreach (NetConnection connection in ClientFrameStates.Keys)
            {
                // Quick and dirty for now - do some nice differentials later
                var message = new GameStateKeyFrame();
                var outgoingMessage = message.ToNetOutgoingMessage(ServerConnection);
                ServerConnection.SendMessage(outgoingMessage, connection, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkServer");
            ServerConnection.Shutdown("NetworkServer shutting down calmly");
        }

        protected void ProcessUnhandledMessageType(NetIncomingMessage message)
        {
            Validate.IsTrue(false,
                $"NetworkServer currently doesn't support messages of the type {message.MessageType}. Message: {message}");
        }

        protected void ProcessError(NetIncomingMessage message)
        {
            Validate.IsTrue(false,
                $"Received IncomingMessage with error type, this shouldn't happen! Message: {message}");
        }

        protected void ProcessConnectionLatencyUpdated(NetIncomingMessage message)
        {
            // TODO: Handle latency updates (currently not supported)
            LOG.Info($"Received LatencyUpdate {message}");
            throw new NotImplementedException();
        }

        protected void ProcessConnectionApproval(NetIncomingMessage message)
        {
            if (ClientFrameStates.ContainsKey(message.SenderConnection))
            {
                // TODO: Metrics
                LOG.Error(
                    $"Denying Connection {message.SenderConnection}, this connection is currently already approved.");
                message.SenderConnection.Deny();
            }

            /*
                Only approve the connection here - wait to instantiate the client's FrameModel until they send our 
                own special ClientApproval packet 
            */
            LOG.Info($"Approving NetworkServer client connection {message.SenderConnection}");
            message.SenderConnection.Approve();
            ProcessData(message);
        }

        protected void ProcessData(NetIncomingMessage message)
        {
            Validate.IsNotNull(message, "Cannot process server data on a null message!");
            var networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            Validate.IsNotNull(networkMessage,
                $"Could not properly format a NetworkMessage from NetIncomingMessage {message}");

            DeMarshall demarshall;

            switch (networkMessage.MessageType)
            {
                case MessageType.CLIENT_CONNECTION_REQUEST:
                    demarshall = HandleClientConnectionRequest;
                    break;
                case MessageType.CLIENT_DATA_DIFF:
                    demarshall = HandleClientDataDiff;
                    break;
                case MessageType.CLIENT_KEY_FRAME:
                    demarshall = HandleClientDataKeyFrame;
                    break;
                case MessageType.SERVER_DATA_DIFF:
                    demarshall = HandleServerDataDiff;
                    break;
                case MessageType.SERVER_DATA_KEYFRAME:
                    demarshall = HandleServerDataKeyframe;
                    break;
                default:
                    demarshall = HandleUnhandledType;
                    break;
            }

            demarshall(networkMessage, message.SenderConnection);
        }

        protected void HandleClientDataDiff(NetworkMessage message, NetConnection connection)
        {
            var clientDataDiffMessage = ConvertMessageType<GameStateDiff>(message);

            // TODO: Nice lerp logic. For now, just ignore client messages

            //var clientConnection = clientDataDiffMessage.Connection;
        }

        protected void HandleClientDataKeyFrame(NetworkMessage message, NetConnection connection)
        {
            // TODO: Nice lerp logic. For now, just ignore client messages
        }

        protected void HandleClientConnectionRequest(NetworkMessage message, NetConnection connection)
        {
            var clientConnectionRequest = ConvertMessageType<ClientConnectionRequest>(message);

            Validate.IsFalse(ClientFrameStates.ContainsKey(connection),
                $"Received ClientConnectionRequest that we're already tracking, this is an issue! Request: {clientConnectionRequest}");

            var frameModel = new FrameModel(DxGame);
            ClientFrameStates.Add(connection, frameModel);
            LOG.Info(
                $"Successfully initialized ClientConnection {connection} for player {clientConnectionRequest.PlayerName}");
        }

        protected void HandleServerDataDiff(NetworkMessage message, NetConnection connection)
        {
            Validate.IsTrue(false, $"Received a Server Data Diff {message} from a client. This should not happen");
        }

        protected void HandleServerDataKeyframe(NetworkMessage message, NetConnection connection)
        {
            Validate.IsTrue(false, $"Received a Server Data Keyframe {message} from a client. This should not happen");
        }

        protected void HandleUnhandledType(NetworkMessage message, NetConnection connection)
        {
            Validate.IsTrue(false, $"Received an unexpected messagetype {message} from a client. This should not happen");
        }
    }
}