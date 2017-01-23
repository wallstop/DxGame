using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Network;
using DxCore.Core.Primitives;
using DxCore.Core.Services.Components;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    public class NetworkService : DxService
    {
        private readonly List<NetworkComponent> connections_ = new List<NetworkComponent>();
        // TODO: Empty checks
        public IEnumerable<AbstractNetworkClient> Clients => connections_.OfType<AbstractNetworkClient>();

        public IEnumerable<AbstractNetworkServer> Servers => connections_.OfType<AbstractNetworkServer>();

        private NetworkProcessor NetworkProcessor { get; set; }

        public void AttachClient(AbstractNetworkClient client)
        {
            WithClient(client);
        }

        public void AttachServer(AbstractNetworkServer server)
        {
            WithServer(server);
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

        // TODO: Get outta here
        public NetworkService WithClient(AbstractNetworkClient client)
        {
            AddNetworkComponent(client);
            return this;
        }

        public NetworkService WithServer(AbstractNetworkServer server)
        {
            // Really? Why not more?
            Validate.Hard.IsEmpty(Servers, () => $"Cannot add {server}. Can only add one server to a NetworkModel");
            AddNetworkComponent(server);
            return this;
        }

        protected void AddNetworkComponent(NetworkComponent netComponent)
        {
            Validate.Hard.IsNotNullOrDefault(netComponent,
                () => $"Cannot add a null/default NetworkComponent to {GetType()}");
            Validate.Hard.IsNotElementOf(connections_, netComponent,
                () => $"Cannot add NetworkComponent {netComponent}. This component already exists in {connections_}");
            connections_.Add(netComponent);
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(NetworkProcessor))
            {
                NetworkProcessor = new NetworkProcessor(connections_);
                Self.AttachComponent(NetworkProcessor);
            }
        }
    }
}