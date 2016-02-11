using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Network;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Models
{
    public class NetworkModel : Model
    {
        private readonly List<NetworkComponent> connections_ = new List<NetworkComponent>();
        // TODO: Empty checks
        public IEnumerable<NetworkClient> Clients => connections_.OfType<NetworkClient>();

        public IEnumerable<NetworkServer> Servers => connections_.OfType<NetworkServer>();

        public override bool ShouldSerialize => false;

        public NetworkModel()
        {
            MessageHandler.EnableAcceptAll();
            MessageHandler.RegisterMessageHandler<Message>(HandleMessageReceived);
        }

        private void HandleMessageReceived(Message message)
        {
            foreach(NetworkComponent networkComponent in connections_)
            {
                networkComponent.MessageHandler.HandleMessage(message);
            }
        }

        public NetworkModel WithClient(NetworkClient client)
        {
            AddNetworkComponent(client);
            return this;
        }

        public NetworkModel WithServer(NetworkServer server)
        {
            // Really? Why not more?
            Validate.IsEmpty(Servers, $"Cannot add {server}. Can only add one server to a NetworkModel");
            AddNetworkComponent(server);
            return this;
        }

        public void AttachServer(NetworkServer server)
        {
            WithServer(server);
        }

        public void AttachClient(NetworkClient client)
        {
            WithClient(client);
        }

        protected override void Update(DxGameTime gameTime)
        {
            foreach(NetworkComponent networkComponent in connections_)
            {
                networkComponent.Process(gameTime);
            }
        }

        protected void AddNetworkComponent(NetworkComponent netComponent)
        {
            Validate.IsNotNullOrDefault(netComponent, $"Cannot add a null/default NetworkComponent to {GetType()}");
            Validate.IsTrue(!connections_.Contains(netComponent),
                $"Cannot add NetworkComponent {netComponent}. This component already exists in {connections_}");
            connections_.Add(netComponent);
        }

        public void ReceiveData(DxGameTime gameTime)
        {
            foreach (NetworkComponent connection in connections_)
            {
                connection.ReceiveData(gameTime);
            }
        }

        public void SendData(DxGameTime gameTime)
        {
            foreach (NetworkComponent connection in connections_)
            {
                connection.SendData(gameTime);
            }
        }

        public void ShutDown()
        {
            foreach (var client in Clients)
            {
                client.Shutdown();
            }

            foreach (var server in Servers)
            {
                server.Shutdown();
            }
        }
    }
}