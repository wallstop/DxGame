using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Basic;
using DXGame.Core.Generators;
using DXGame.Core.Lerp;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Network;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
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
        private static readonly TimeSpan TICK_RATE = TimeSpan.FromSeconds(1.0 / 30); // 30 FPS
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public NetServer ServerConnection => Connection as NetServer;
        public Dictionary<NetConnection, ServerEventTracker> ClientFrameStates { get; }

        /* 
            In addition to keeping track of unique diffs for each Connection, we need to 
            keep track of all global diffs for *NEW* connections. This way, whenever a 
            client connects, we can shit out "the world thus far" to them in a 
            succinct manner.
        */
        private readonly ServerEventTracker baseEventTracker_ = new ServerEventTracker();

        private TimeSpan lastSent_ = TimeSpan.Zero;

        public NetworkServer()
        {
            ClientFrameStates = new Dictionary<NetConnection, ServerEventTracker>();
            MessageHandler.EnableAcceptAll(HandleMessage);
        }

        protected override void InternalSendData(DxGameTime gameTime)
        {
            // TODO: Function-ize
            foreach(KeyValuePair<NetConnection, ServerEventTracker> connectionEventTrackingPair in ClientFrameStates)
            {
                List<Message> events = connectionEventTrackingPair.Value.RetrieveEvents();
                events.RemoveAll(message =>
                {
                    try
                    {
                        Serializer<Message>.BinarySerialize(message);
                        return false;
                    }
                    catch(Exception)
                    {
                        // TODO: This is shitty and bad, do something else
                        return true;
                    }
                });
                EventStream eventStream = new EventStream(events);
                NetOutgoingMessage outgoingMessage = eventStream.ToNetOutgoingMessage(ServerConnection);
                ServerConnection.SendMessage(outgoingMessage, connectionEventTrackingPair.Key,
                    NetDeliveryMethod.ReliableOrdered, REQUIRED_MESSAGE_CHANNEL);
            }

            List<IDxVectorLerpable> dxVectorLerpables =
                DxGame.Instance.DxGameElements.OfType<IDxVectorLerpable>().ToList();

            foreach(IDxVectorLerpable dxVectorLerpable in dxVectorLerpables)
            {
                DxVectorLerpMessage dxVectorLerpMessage = new DxVectorLerpMessage(dxVectorLerpable.Id,
                    dxVectorLerpable.LerpValueSnapshot);
                NetOutgoingMessage outgoingLerpMessage = dxVectorLerpMessage.ToNetOutgoingMessage(ServerConnection);
                foreach(NetConnection clientConnection in ClientFrameStates.Keys)
                {
                    /* Don't really care if the client picks these up... */
                    ServerConnection.SendMessage(outgoingLerpMessage, clientConnection, NetDeliveryMethod.Unreliable,
                        LERP_DATA_CHANNEL);
                }
            }

            lastSent_ = gameTime.TotalGameTime;
        }

        protected override void InitializeNetworkMessageListeners()
        {
            networkMessageHandlers_[typeof(ClientConnectionRequest)] = HandleClientConnectionRequest;
            networkMessageHandlers_[typeof(ClientTimeSynchronizationRequest)] = HandleClientTimeSynchronizationRequest;
        }

        public override TimeSpan TickRate => TICK_RATE;

        public override NetworkComponent WithConfiguration(NetPeerConfiguration configuration)
        {
            // Make sure we enable Approvals so we can accept Client Connections
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Connection = new NetServer(configuration);
            return this;
        }

        private void HandleMessage(Message message)
        {
            baseEventTracker_.Handler.HandleTypedMessage(message);
            foreach(ServerEventTracker eventTracker in ClientFrameStates.Values)
            {
                eventTracker.Handler.HandleTypedMessage(message);
            }
        }

        public override void EstablishConnection()
        {
            ServerConnection.Start();
        }

        public override void RouteDataOnMessageType(NetIncomingMessage incomingMessage, DxGameTime gameTime)
        {
            // TODO: Deal with gameTime
            switch(incomingMessage.MessageType)
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
                default:
                    LOG.Info(
                        $"Received MessageType {incomingMessage.MessageType}. Currently not handling. ({incomingMessage})");
                    // TODO: Handle
                    break;
                //ProcessUnhandledMessageType(incomingMessage);
                //break;
            }
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkServer");
            ServerConnection.Shutdown("NetworkServer shutting down calmly");
            base.Shutdown();
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
            if(ClientFrameStates.ContainsKey(message.SenderConnection))
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

        protected void HandleClientDataDiff(NetworkMessage message, NetConnection connection)
        {
            // TODO: Nice lerp logic. For now, just ignore client messages

            //var clientConnection = clientDataDiffMessage.Connection;
        }

        protected void HandleClientDataKeyFrame(NetworkMessage message, NetConnection connection)
        {
            // TODO: Nice lerp logic. For now, just ignore client messages
        }

        protected void HandleClientTimeSynchronizationRequest(NetworkMessage message, NetConnection connection)
        {
            ClientTimeSynchronizationRequest timeSynchronizationRequest =
                ConvertMessageType<ClientTimeSynchronizationRequest>(message);

            ServerTimeUpdate timeUpdate = new ServerTimeUpdate(timeSynchronizationRequest.ClientSideGameTime,
                DxGame.Instance.CurrentTime);
            NetOutgoingMessage outgoingTimeUpdate = timeUpdate.ToNetOutgoingMessage(ServerConnection);
            ServerConnection.SendMessage(outgoingTimeUpdate, connection, NetDeliveryMethod.Unreliable,
                TIME_SYNCHRONIZATION_CHANNEL);
        }

        protected void HandleClientConnectionRequest(NetworkMessage message, NetConnection connection)
        {
            ClientConnectionRequest clientConnectionRequest = ConvertMessageType<ClientConnectionRequest>(message);

            Validate.IsFalse(ClientFrameStates.ContainsKey(connection),
                $"Received ClientConnectionRequest that we're already tracking, this is an issue! Request: {clientConnectionRequest}");

            ServerEventTracker eventTracker = new ServerEventTracker(baseEventTracker_);
            ClientFrameStates.Add(connection, eventTracker);
            LOG.Info(
                $"Successfully initialized ClientConnection {connection} for player {clientConnectionRequest.PlayerName}");

            MapModel mapModel = DxGame.Instance.Model<MapModel>();
            PlayerGenerator playerGenerator = new PlayerGenerator(mapModel.RandomSpawnLocation.Center.ToDxVector2());

            AbstractCommandComponent networkPlayerCommand = new SimpleRelayingCommandComponent();

            playerGenerator.GeneratePlayer(networkPlayerCommand);
            //GameObject player
            // 
        }
    }
}