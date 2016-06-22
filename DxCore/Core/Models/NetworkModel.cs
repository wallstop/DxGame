using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Network;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Models
{
    public class NetworkModel : Model
    {
        private readonly List<NetworkComponent> connections_ = new List<NetworkComponent>();
        // TODO: Empty checks
        public IEnumerable<AbstractNetworkClient> Clients => connections_.OfType<AbstractNetworkClient>();

        public IEnumerable<AbstractNetworkServer> Servers => connections_.OfType<AbstractNetworkServer>();

        public override bool ShouldSerialize => false;

        // TODO: Get outta here
        public NetworkModel WithClient(AbstractNetworkClient client)
        {
            AddNetworkComponent(client);
            return this;
        }

        public NetworkModel WithServer(AbstractNetworkServer server)
        {
            // Really? Why not more?
            Validate.Hard.IsEmpty(Servers, $"Cannot add {server}. Can only add one server to a NetworkModel");
            AddNetworkComponent(server);
            return this;
        }

        public void AttachServer(AbstractNetworkServer server)
        {
            WithServer(server);
        }

        public void AttachClient(AbstractNetworkClient client)
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
            Validate.Hard.IsNotNullOrDefault(netComponent, $"Cannot add a null/default NetworkComponent to {GetType()}");
            Validate.Hard.IsTrue(!connections_.Contains(netComponent),
                $"Cannot add NetworkComponent {netComponent}. This component already exists in {connections_}");
            connections_.Add(netComponent);
        }

        public void ReceiveData(DxGameTime gameTime)
        {
            foreach(NetworkComponent connection in connections_)
            {
                connection.ReceiveData(gameTime);
            }
        }

        public void SendData(DxGameTime gameTime)
        {
            foreach(NetworkComponent connection in connections_)
            {
                connection.SendData(gameTime);
            }
        }

        public void ShutDown()
        {
            foreach(var client in Clients)
            {
                client.Shutdown();
            }

            foreach(var server in Servers)
            {
                server.Shutdown();
            }
        }
    }
}