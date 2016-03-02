using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DXGame.Core.Components.Basic;
using DXGame.Core.Lerp;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Network;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Lerp;
using DXGame.Main;
using Lidgren.Network;
using NLog;

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
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        protected NetworkClientConfig ClientConfig { get; set; }
        public NetClient ClientConnection => Connection as NetClient;

        private readonly LerpDataCollector<DxVector2> dxVector2LerpData_ = new LerpDataCollector<DxVector2>(); 

        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private List<Message> messagesToBroadcast_ = new List<Message>(); 

        public NetworkClient WithNetworkClientConfig(NetworkClientConfig configuration)
        {
            Validate.IsNotNullOrDefault(configuration,
                "Cannot create a NetworkClient with a null/default NetworkClientConfig");
            ClientConfig = configuration;
            return this;
        }

        public override NetworkComponent WithConfiguration(NetPeerConfiguration configuration)
        {
            Validate.IsNotNullOrDefault(configuration,
                "Cannot create a NetworkClient with a null/default NetPeerConfiguration");
            Connection = new NetClient(configuration);
            return this;
        }

        public override void EstablishConnection()
        {
            ClientConnection.Start();

            ClientConnectionRequest request = new ClientConnectionRequest(ClientConfig.PlayerName);
            NetOutgoingMessage outMessage = request.ToNetOutgoingMessage(ClientConnection);
            ClientConnection.Connect(ClientConfig.IpAddress, ClientConfig.Port, outMessage);
        }

        public override void RouteDataOnMessageType(NetIncomingMessage message, DxGameTime gameTime)
        {
            // TODO: Deal with gameTime
            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    ProcessData(message);
                    break;
                case NetIncomingMessageType.DebugMessage:
                    ProcessDebugMessage(message);
                    break;
                default:
                    LOG.Info(
                        $"Received NetMessage of type {message.MessageType}. Currently not handling this. ({message.MessageContents()})");
                    break;
            }
        }

        protected void ProcessDebugMessage(NetIncomingMessage message)
        {
            // TODO
        }

        public override void SendData(DxGameTime gameTime)
        {
            // TODO: send message stream of input to server (pretty ez)
        }

        protected override void PostReceiveData(DxGameTime gameTime)
        {
            List<Message> messagesToBroadcast;
            using(new CriticalRegion(lock_, CriticalRegion.LockType.Write))
            {
                messagesToBroadcast = messagesToBroadcast_;
                messagesToBroadcast_ = new List<Message>();
            }

            foreach(Message message in messagesToBroadcast)
            {
                DxGame.Instance.BroadcastUntypedMessage(message);
            }

                // TODO: Make this not shit (Keep our own id -> component mapping?)
            List<IDxVectorLerpable> dxVectorLerpables = DxGame.Instance.Components.OfType<IDxVectorLerpable>().ToList();
            foreach(IDxVectorLerpable dxVectorLerpable in dxVectorLerpables)
            {
                UniqueId entityId = dxVectorLerpable.Id;
                LerpData<DxVector2> dxVectorLerpData;
                if(dxVector2LerpData_.TryGetLerpData(entityId, out dxVectorLerpData))
                {
                    dxVectorLerpable.Lerp(dxVectorLerpData.OldValue, dxVectorLerpData.NewValue, dxVectorLerpData.OldTime, dxVectorLerpData.NewTime, gameTime.TotalGameTime);
                }
            }
            base.PostReceiveData(gameTime);
        }

        protected override void InitializeNetworkMessageListeners()
        {
            networkMessageHandlers_[typeof(EventStream)] = HandleEventStream;
            networkMessageHandlers_[typeof(DxVectorLerpMessage)] = HandleDxVectorLerpMessage;
        }

        private void HandleEventStream(NetworkMessage message, NetConnection netConnection)
        {
            EventStream eventStream = ConvertMessageType<EventStream>(message);
            using(new CriticalRegion(lock_, CriticalRegion.LockType.Write))
            {
                messagesToBroadcast_.AddRange(eventStream.Messages);   
            }
        }

        private void HandleDxVectorLerpMessage(NetworkMessage message, NetConnection netConnection)
        {
            DxVectorLerpMessage dxVectorLerpMessage = ConvertMessageType<DxVectorLerpMessage>(message);
            dxVector2LerpData_.UpdateLerpData(dxVectorLerpMessage.EntityId, dxVectorLerpMessage.CurrentLerpValue, dxVectorLerpMessage.TimeStamp.TotalGameTime);
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkClient");
            ClientConnection.Shutdown("NetworkClient shutting down calmly");
            base.Shutdown();
        }
    }
}