using System;
using System.Collections.Generic;
using System.Threading;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Network;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class NetworkMessageMarshalling
    {
        private static readonly DxGame GAME = new DxGame();

        private static readonly PositionalComponent POSITION = new PositionalComponent(GAME).WithPosition(3, 4);

        private static readonly PhysicsComponent PHYSICS =
            new PhysicsComponent(GAME).WithPositionalComponent(POSITION)
                .WithAcceleration(new Vector2(300, 100))
                .WithVelocity(new Vector2(100, 104));

        private const string APP_ID = "NetworkMarshallTest";

        private NetServer server_;

        private NetClient client_;

        [SetUp]
        public void Init()
        {
            NetworkMarshalling.Init();
            server_ = GenerateServer();
            client_ = GenerateClient();

            NetIncomingMessage message = server_.ReadMessage();
            if (message == null)
            {
                throw new Exception("Could not connect to local server! Something is seriously wrong.");
            }

            if (message.MessageType != NetIncomingMessageType.ConnectionApproval)
            {
                throw new Exception(String.Format("Expected a connection approval, received {0}", message.MessageType));
            }

            message.SenderConnection.Approve();
        }

        private static NetServer GenerateServer()
        {
            var config = new NetPeerConfiguration(APP_ID) { Port = 800, MaximumConnections = 2 };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            var server = new NetServer(config);
            server.Start();
            return server;
        }

        private static NetClient GenerateClient()
        {
            var config = new NetPeerConfiguration(APP_ID);
            var client = new NetClient(config);
            var message = client.CreateMessage();
            client.Start();
            client.Connect("127.0.0.1", 800, message);
            Thread.Sleep(1000);

            return client;
        }


        [Test]
        public void MarshallVector2()
        {
            var vector2 = new Vector2(300.4f, 102.33f);
            var outMessage = client_.CreateMessage();
            
            NetworkMarshaller<Vector2>.Write(vector2, outMessage);
            var outBits = outMessage.LengthBits;
            client_.SendMessage(outMessage, NetDeliveryMethod.ReliableOrdered);

            // Make sure we get the message
            Thread.Sleep(1000);

            NetIncomingMessage inMessage = null;
            do
            {
                inMessage = server_.ReadMessage();
            } while (inMessage == null || inMessage.LengthBits != outBits);

            var outVector2 = NetworkMarshaller<Vector2>.Read(inMessage);
            Assert.AreEqual(vector2, outVector2);
        }
    }
}