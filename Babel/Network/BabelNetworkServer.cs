using Babel.Generators;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Network;
using DxCore.Core.Messaging.Network;
using DxCore.Core.Network;
using DxCore.Core.Services;
using DxCore.Core.Utils.Validate;
using Lidgren.Network;

namespace Babel.Network
{
    public class BabelNetworkServer : AbstractNetworkServer
    {
        public BabelNetworkServer(NetPeerConfiguration configuration) : base(configuration) {}

        protected override void InitializeNetworkMessageListeners()
        {
            networkMessageHandlers_[typeof(ClientConnectionRequest)] = HandleClientConnectionRequest;
            networkMessageHandlers_[typeof(ClientTimeSynchronizationRequest)] = HandleClientTimeSynchronizationRequest;
            networkMessageHandlers_[typeof(ClientCommands)] = HandleClientCommands;
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
                DxGame.Instance.CurrentUpdateTime);
            NetOutgoingMessage outgoingTimeUpdate = timeUpdate.ToNetOutgoingMessage(ServerConnection);
            ServerConnection.SendMessage(outgoingTimeUpdate, connection, NetDeliveryMethod.Unreliable);
        }

        protected void HandleClientCommands(NetworkMessage message, NetConnection connection)
        {
            ClientEventTracker eventTracker;
            if(!ClientFrameStates.TryGetValue(connection, out eventTracker))
            {
                LOG.Error($"Encountered unexpected client commands from {connection}");
                return;
            }

            ClientCommands clientCommands = ConvertMessageType<ClientCommands>(message);
            eventTracker.PlayerCommand.RelayCommands(clientCommands.ClientCommandments);
        }

        protected void HandleClientConnectionRequest(NetworkMessage message, NetConnection connection)
        {
            ClientConnectionRequest clientConnectionRequest = ConvertMessageType<ClientConnectionRequest>(message);

            Validate.Hard.IsFalse(ClientFrameStates.ContainsKey(connection), () =>
                $"Received ClientConnectionRequest that we're already tracking, this is an issue! Request: {clientConnectionRequest}");

            ServerEventTracker eventTracker = new ServerEventTracker(baseEventTracker_);

            MapService mapService = DxGame.Instance.Service<MapService>();
            BabelPlayerGenerator playerGenerator =
                new BabelPlayerGenerator(mapService.RandomSpawnLocation.Center);

            SimpleRelayingCommandComponent networkPlayerCommand = new SimpleRelayingCommandComponent(TickRate);

            ClientEventTracker clientEventTracker = new ClientEventTracker(eventTracker, networkPlayerCommand);
            ClientFrameStates.Add(connection, clientEventTracker);
            LOG.Info($"Successfully initialized ClientConnection {connection} for player {clientConnectionRequest}");

            GameObject player = playerGenerator.GeneratePlayer(networkPlayerCommand, false);
            player.Create();
            UpdateActivePlayer updateActivePlayerRequest = new UpdateActivePlayer(player.Id);
            eventTracker.AttachNetworkMessage(updateActivePlayerRequest);
        }
    }
}
