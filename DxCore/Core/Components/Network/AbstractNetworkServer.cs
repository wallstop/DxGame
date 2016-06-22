using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Lerp;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Network;
using DxCore.Core.Network;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Lidgren.Network;
using NLog;

namespace DxCore.Core.Components.Network
{
    // TODO: Make moar better abstract
    public abstract class AbstractNetworkServer : NetworkComponent
    {
        protected static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public NetServer ServerConnection => Connection as NetServer;
        public Dictionary<NetConnection, ClientEventTracker> ClientFrameStates { get; }

        /* 
            In addition to keeping track of unique diffs for each Connection, we need to 
            keep track of all global diffs for *NEW* connections. This way, whenever a 
            client connects, we can shit out "the world thus far" to them in a 
            succinct manner.
        */
        protected readonly ServerEventTracker baseEventTracker_ = new ServerEventTracker();

        protected AbstractNetworkServer(NetPeerConfiguration configuration) : base(configuration)
        {
            ClientFrameStates = new Dictionary<NetConnection, ClientEventTracker>();
        }

        protected override void InternalSendData(DxGameTime gameTime)
        {
            InternalDefaultSendMessageStream(gameTime);
            InternalDefaultSendLerpData(gameTime);
        }

        protected void InternalDefaultSendMessageStream(DxGameTime gameTime)
        {
            foreach(KeyValuePair<NetConnection, ClientEventTracker> connectionEventTrackingPair in ClientFrameStates)
            {
                List<Message> events = connectionEventTrackingPair.Value.ServerEventTracker.RetrieveEvents();

                int removedCount = events.RemoveAll(message =>
                {
                    try
                    {
                        Serializer<Message>.BinarySerialize(message);
                        return false;
                    }
                    catch(Exception e)
                    {
                        // TODO: This is shitty and bad, do something else
                        LOG.Error(e, "Failed to process {0}", message);
                        return true;
                    }
                });
                LOG.Info("Failed to process {0} messages, {1} remaining", removedCount, events.Count());
                EventStream eventStream = new EventStream(events);
                NetOutgoingMessage outgoingMessage = eventStream.ToNetOutgoingMessage(ServerConnection);
                ServerConnection.SendMessage(outgoingMessage, connectionEventTrackingPair.Key,
                    NetDeliveryMethod.ReliableOrdered, REQUIRED_MESSAGE_CHANNEL);

                List<NetworkMessage> clientSpecificMessages;
                if(connectionEventTrackingPair.Value.ServerEventTracker.RetrieveNetworkEvents(out clientSpecificMessages))
                {
                    foreach(NetworkMessage clientMessage in clientSpecificMessages)
                    {
                        NetOutgoingMessage outgoingClientMessage = clientMessage.ToNetOutgoingMessage(ServerConnection);
                        ServerConnection.SendMessage(outgoingClientMessage, connectionEventTrackingPair.Key,
                            NetDeliveryMethod.ReliableOrdered, CLIENT_SPECIFIC_UPDATES);
                    }
                }
            }
        }

        protected void InternalDefaultSendLerpData(DxGameTime gameTime)
        {
            List<IDxVectorLerpable> dxVectorLerpables =
                DxGame.Instance.DxGameElements.OfType<IDxVectorLerpable>().ToList();

            foreach(IDxVectorLerpable dxVectorLerpable in dxVectorLerpables)
            {
                DxVectorLerpMessage dxVectorLerpMessage = new DxVectorLerpMessage(dxVectorLerpable.Id,
                    dxVectorLerpable.LerpValueSnapshot);
                NetOutgoingMessage outgoingLerpMessage = dxVectorLerpMessage.ToNetOutgoingMessage(ServerConnection);
                foreach(NetConnection clientConnection in ClientFrameStates.Keys)
                {
                    /* Don't really care if the client picks these up... (but do we care about order? not right now...) */
                    ServerConnection.SendMessage(outgoingLerpMessage, clientConnection, NetDeliveryMethod.Unreliable);
                }
            }
        }

        protected abstract override void InitializeNetworkMessageListeners();

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
            Validate.Hard.IsTrue(false,
                $"NetworkServer currently doesn't support messages of the type {message.MessageType}. Message: {message}");
        }

        protected void ProcessError(NetIncomingMessage message)
        {
            Validate.Hard.IsTrue(false,
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
    }
}
