﻿using System;
using System.Collections;
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

        public NetworkClient(DxGame game)
            : base(game)
        {
        }

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

            ClientConnectionRequest request = new ClientConnectionRequest {PlayerName = ClientConfig.PlayerName};
            var outMessage = request.ToNetOutgoingMessage(ClientConnection);

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
                        $"Received NetMessage of type {message.MessageType}. Currently not handling this. ({StringUtils.GetString(message.PeekDataBuffer())})");
                    break;
            }
        }

        protected void ProcessDebugMessage(NetIncomingMessage message)
        {

        }

        protected void ProcessData(NetIncomingMessage message)
        {
            Validate.IsNotNull(message, "Cannot process server data on a null message!");
            NetworkMessage networkMessage = null;
            try
            {
                networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            }
            catch (Exception e)
            {
                LOG.Error(e, "Caught unexpected exception while attempting to process network data message");
            }
            finally
            {
                Validate.IsNotNull(networkMessage,
                    $"Could not properly format a NetworkMessage from NetIncomingMessage {networkMessage}");
            }

            switch (networkMessage.MessageType)
            {
                case MessageType.SERVER_DATA_KEYFRAME:
                    HandleServerDataKeyFrame(networkMessage);
                    break;
                default:
                    LOG.Info(
                        $"Received NetMessage of type {message.MessageType}. Currently not handling this. ({StringUtils.GetString(message.PeekDataBuffer())})");
                    break;
            }
        }

        protected void HandleServerDataKeyFrame(NetworkMessage message)
        {
            var serverDataKeyFrame = ConvertMessageType<GameStateKeyFrame>(message);

            Predicate<object> shouldSerialize = entity =>
            {
                var component = entity as Component;
                return component != null && component.ShouldSerialize;
            };
            // TODO: Faster way of dumping state
            DxGame.DxGameElements.Remove(shouldSerialize);
            foreach (var element in serverDataKeyFrame.GameElements)
            {
                DxGame.DxGameElements.Add(element);
            }
            DxGame.NewGameElements.Clear();
            foreach (var element in serverDataKeyFrame.NewGameElements)
            {
                DxGame.NewGameElements.Add(element);
            }
            DxGame.RemovedGameElements.Clear();
            foreach (var element in serverDataKeyFrame.RemovedGameEleemnts)
            {
                DxGame.RemovedGameElements.Add(element);
            }
        }

        public override void SendData(DxGameTime gameTime)
        {
            // TODO:
        }

        public override void Shutdown()
        {
            LOG.Info("Shutting down NetworkClient");
            ClientConnection.Shutdown("NetworkClient shutting down calmly");
            base.Shutdown();
        }
    }
}