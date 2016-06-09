using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Lerp;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Game;
using DxCore.Core.Messaging.Network;
using DxCore.Core.Models;
using DxCore.Core.Network;
using DXGame.Core;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Lerp;
using Lidgren.Network;
using NLog;

namespace DxCore.Core.Components.Network
{
    public struct NetworkClientConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string PlayerName { get; set; }
    }

    public class NetworkClient : NetworkComponent
    {
        private static readonly TimeSpan TICK_RATE = TimeSpan.FromSeconds(1.0 / 60); // 60 FPS
        private static readonly TimeSpan TIME_SYNCHRONIZATION_RATE = TimeSpan.FromSeconds(1);
        private TimeSpan lastSynchronizedTime_ = TimeSpan.Zero;

        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        protected NetworkClientConfig ClientConfig { get; set; }
        public NetClient ClientConnection => Connection as NetClient;

        private readonly LerpDataCollector<DxVector2> dxVector2LerpData_ = new LerpDataCollector<DxVector2>();

        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private List<Message> messagesToBroadcast_ = new List<Message>();

        private UniqueId playerId_;

        public NetworkClient WithNetworkClientConfig(NetworkClientConfig configuration)
        {
            Validate.IsNotNullOrDefault(configuration,
                "Cannot create a NetworkClient with a null/default NetworkClientConfig");
            ClientConfig = configuration;
            return this;
        }

        public override TimeSpan TickRate => TICK_RATE;

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
            switch(message.MessageType)
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

        protected override void InternalSendData(DxGameTime gameTime)
        {
            SendTimeSynchronizationRequest(gameTime);
            /* 
                TODO: Accrue input events over the period since the NetworkClient has ticked - 
                we don't have a 1-1 mapping between game ticks and network send ticks (its totally arbitrary) 
            */
            SendClientCommandments(gameTime);
        }

        private void SendTimeSynchronizationRequest(DxGameTime gameTime)
        {
            if(ReferenceEquals(Connection, null))
            {
                return;
            }
            if(ReferenceEquals(ClientConnection.ServerConnection, null))
            {
                return;
            }

            if(lastSynchronizedTime_ + TIME_SYNCHRONIZATION_RATE < gameTime.TotalGameTime)
            {
                ClientTimeSynchronizationRequest clientSynchronizationRequest =
                    new ClientTimeSynchronizationRequest(gameTime);
                NetOutgoingMessage outgoingSyncronizationRequest =
                    clientSynchronizationRequest.ToNetOutgoingMessage(Connection);
                Connection.SendMessage(outgoingSyncronizationRequest, ClientConnection.ServerConnection,
                    NetDeliveryMethod.Unreliable);
                lastSynchronizedTime_ = gameTime.TotalGameTime;
            }
        }

        private void SendClientCommandments(DxGameTime gameTime)
        {
            if(ReferenceEquals(Connection, null))
            {
                return;
            }

            List<KeyboardEvent> keyboardEvents = PlayerInputListener.RipEventsFromLocalInputModel();
            List<Commandment> clientCommandments = PlayerInputListener.DetermineCommandmentsFor(keyboardEvents);
            if(clientCommandments.Any())
            {
                ClientCommands clientCommands = new ClientCommands(clientCommandments);
                NetOutgoingMessage outgoingClientCommands = clientCommands.ToNetOutgoingMessage(Connection);
                Connection.SendMessage(outgoingClientCommands, ClientConnection.ServerConnection,
                    NetDeliveryMethod.ReliableOrdered, PLAYER_INPUT_CHANNEL);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            List<IDxVectorLerpable> dxVectorLerpables =
                DxGame.Instance.DxGameElements.OfType<IDxVectorLerpable>().ToList();
            foreach(IDxVectorLerpable dxVectorLerpable in dxVectorLerpables)
            {
                UniqueId lerpableId = dxVectorLerpable.Id;
                LerpData<DxVector2> lerpData;
                if(dxVector2LerpData_.TryGetLerpData(lerpableId, out lerpData))
                {
                    dxVectorLerpable.Lerp(lerpData.OldValue, lerpData.NewValue, lerpData.OldTime, lerpData.NewTime,
                        gameTime.TotalGameTime);
                }
            }

            if(!ReferenceEquals(playerId_, null))
            {
                List<GameObject> maybePlayers = DxGame.Instance.DxGameElements.OfType<GameObject>().ToList();
                foreach(GameObject maybePlayer in maybePlayers)
                {
                    if(Objects.Equals(maybePlayer.Id, playerId_))
                    {
                        Player clientSideActivePlayer = Player.PlayerFrom(maybePlayer, "Bartholomew");
                        PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
                        playerModel.WithActivePlayer(clientSideActivePlayer);
                        playerId_ = null;
                        break;
                    }
                }
            }

            base.Update(gameTime);
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
                message.EmitUntyped();
            }
            base.PostReceiveData(gameTime);
        }

        protected override void InitializeNetworkMessageListeners()
        {
            networkMessageHandlers_[typeof(EventStream)] = HandleEventStream;
            networkMessageHandlers_[typeof(DxVectorLerpMessage)] = HandleDxVectorLerpMessage;
            networkMessageHandlers_[typeof(ServerTimeUpdate)] = HandleServerTimeUpdate;
            networkMessageHandlers_[typeof(UpdateActivePlayer)] = HandleUpdateActivePlayer;
        }

        private void HandleEventStream(NetworkMessage message, NetConnection netConnection)
        {
            EventStream eventStream = ConvertMessageType<EventStream>(message);
            using(new CriticalRegion(lock_, CriticalRegion.LockType.Write))
            {
                messagesToBroadcast_.AddRange(eventStream.Messages);
            }
        }

        private void HandleServerTimeUpdate(NetworkMessage message, NetConnection netConnection)
        {
            ServerTimeUpdate serverTimeUpdate = ConvertMessageType<ServerTimeUpdate>(message);
            DxGameTime currentTime = DxGame.Instance.CurrentTime;

            double roundTripLatencyInMillis =
                (currentTime.TotalGameTime - serverTimeUpdate.ClientGameTime).TotalMilliseconds;
            double oneWayLatency = roundTripLatencyInMillis / 2;

            /*
                Here's the deal: 
                
                We can get a perfect "clock now is clock on server" if we do some math like
                (serverTime - clientTime). This takes into account 1 way latency.

                However, I'm currently not convinced that's what we want. If we're doing client 
                side prediction, unless we account for latency there, what we actually want is
                (serverTime - clientTime - oneWayLatency). I think. Pretty sure this will allow us
                to have something like

                Client: TimeZ - EventX
                Client->Server -OneWayLatency-
                Server: TimeZ - Client said EventX
                Server->Client -OneWayLatency-
                Client: Yea, cool, server told me I did the thing I thought I did, WE'RE ALL GOOD
            */
            double offsetInMillis = serverTimeUpdate.ServerGameTime.TotalMilliseconds -
                                    serverTimeUpdate.ClientGameTime.TotalMilliseconds - oneWayLatency;

            // TODO: Feed this into a PID controller or some shit
            LOG.Info(
                $"Skew: {offsetInMillis} millis, Average one way latency of {oneWayLatency} millis for timestamp: {serverTimeUpdate.ClientGameTime} (currently {DxGame.Instance.CurrentTime.TotalGameTime}");

            TimeSkewRequest timeSkewRequest = new TimeSkewRequest(offsetInMillis);
            timeSkewRequest.Emit();
        }

        private void HandleDxVectorLerpMessage(NetworkMessage message, NetConnection netConnection)
        {
            DxVectorLerpMessage dxVectorLerpMessage = ConvertMessageType<DxVectorLerpMessage>(message);
            dxVector2LerpData_.UpdateLerpData(dxVectorLerpMessage.EntityId, dxVectorLerpMessage.CurrentLerpValue,
                dxVectorLerpMessage.TimeStamp.TotalGameTime);
        }

        private void HandleUpdateActivePlayer(NetworkMessage message, NetConnection connection)
        {
            UpdateActivePlayer updateActivePlayer = ConvertMessageType<UpdateActivePlayer>(message);
            playerId_ = updateActivePlayer.PlayerId;
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkClient");
            ClientConnection.Shutdown("NetworkClient shutting down calmly");
            base.Shutdown();
        }
    }
}