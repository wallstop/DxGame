﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Models
{
    public class NetworkModel : GameComponentCollection
    {
        protected List<NetworkComponent> connections_ = new List<NetworkComponent>();

        // TODO: Empty checks
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
            AddNetworkComponent(client);
            return this;
        }

        public NetworkModel WithServer(NetworkServer server)
        {
            Debug.Assert(!Servers.Any(), "Can only add one server to a NetworkModel!"); // Really? Why not more?
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

        protected void AddNetworkComponent(NetworkComponent netComponent)
        {
            GenericUtils.CheckNullOrDefault(netComponent, "Cannot add a null/default NetworkComponent to NetworkModel");
            Debug.Assert(!connections_.Contains(netComponent),
                "Cannot add a NetworkComponent that already exists in the NetworkModel");
            connections_.Add(netComponent);
        }

        public void ReceiveData(GameTime gameTime)
        {
            foreach (NetworkComponent connection in connections_)
            {
                connection.ReceiveData(gameTime);
            }
        }

        public void SendData(GameTime gameTime)
        {
            foreach (NetworkComponent connection in connections_)
            {
                connection.SendData(gameTime);
            }
        }
    }
}