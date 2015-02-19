using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Models
{
    public class NetworkModel : GameComponentCollection
    {
        protected List<NetworkComponent> connections_ = new List<NetworkComponent>();


        public IEnumerable<NetworkClient> Clients
        {
            get { return connections_.OfType<NetworkClient>(); }
        }

        public IEnumerable<NetworkServer> Servers
        {
            get { return connections_.OfType<NetworkServer>(); }
        }

        public NetworkModel(DxGame game) 
            : base(game)
        {
        }

        public NetworkModel WithClient(NetworkClient client)
        {
            AddNetPeer(client);
            return this;
        }

        public NetworkModel WithServer(NetworkClient server)
        {
            Debug.Assert(!Servers.Any(), "Can only add one server to a NetworkModel!"); // Really? Why not more?
            AddNetPeer(server);
            return this;
        }

        protected void AddNetPeer(NetworkComponent netPeer)
        {
            GenericUtils.CheckNullOrDefault(netPeer, "Cannot add a null/default NetworkComponent to NetworkModel");
            Debug.Assert(!connections_.Contains(netPeer), "Cannot add a NetworkComponent that already exists in the NetworkModel");
            connections_.Add(netPeer);
        }

        public void ReceiveData()
        {
            if (!Servers.Any())
            {
                return;
            }

            foreach (NetworkServer server in Servers)
            {
                
            }
            
        }

        public void SendData()
        {
            
        }



    }
}
