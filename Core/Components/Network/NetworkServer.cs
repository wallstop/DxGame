using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;
using Lidgren.Network;

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
                var message = new GameStateKeyFrame
                {
                    Components = DxGame.DxComponents.Components().Where(n => !(n is SimplePlayerInputComponent)).ToList(),
                    GameTime = gameTime,
                    MessageType = MessageType.SERVER_DATA_KEYFRAME
                };
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
            ProcessData(message);
        }

        protected void ProcessData(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot process server data on a null message!");
            var networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            GenericUtils.CheckNull(networkMessage,
                "Could not properly format a NetworkMessage from the NetIncomingMessage");

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

            if (ClientFrameStates.ContainsKey(connection))
            {
                GenericUtils.HardFail(LOG,
                    String.Format(
                        "Received ClientConnectionRequest that we're already tracking, this is an issue! Request: {0}",
                        clientConnectionRequest));
                return;
            }

            var frameModel = new FrameModel(DxGame);
            ClientFrameStates.Add(connection, frameModel);
            LOG.Info(String.Format("Successfully initialized ClientConnection {0} for player {1}", connection,
                clientConnectionRequest.PlayerName));
        }

        protected void HandleServerDataDiff(NetworkMessage message, NetConnection connection)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received a Server Data Diff {0} from a client. This should not happen",
                    message));
        }

        protected void HandleServerDataKeyframe(NetworkMessage message, NetConnection connection)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received a Server Data Keyframe {0} from a client. This should not happen",
                    message));
        }

        protected void HandleUnhandledType(NetworkMessage message, NetConnection connection)
        {
            GenericUtils.HardFail(LOG,
                String.Format("Received an unexpected messagetype {0} from a client. This should not happen",
                    message));
        }
    }
}