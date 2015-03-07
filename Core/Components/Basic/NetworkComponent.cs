using System;
using System.Collections.Concurrent;
using System.Threading;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    /*
        Basic networked component. In general, there should really only be two types of these - Client & Server
    */

    public abstract class NetworkComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkComponent));

        /*
            Make sure you know what you're doing when you get/set this. 
            This Connection listener should only be set via Connection setter and never gotten
        */
        protected Thread ConnectionListener { get; set; }
        protected ConcurrentQueue<NetIncomingMessage> MessageQueue { get; set; }

        protected void ReadFromConnection()
        {
            TimeSpan sleepTime = DxGame.TargetElapsedTime;
            try
            {
                while (true)
                {
                    NetIncomingMessage message;
                    while ((message = Connection.ReadMessage()) != null)
                    {
                        MessageQueue.Enqueue(message);
                    }
                    Thread.Sleep(sleepTime);
                }
            }
            catch (ThreadInterruptedException e)
            {
                LOG.Info(String.Format("Shutting down reader for Connection {0}", Connection), e);
            }
        }

        private NetPeer connection_;

        /*
            The Connection property will attempt to terminate the current ConnectionListener thread
            when set and spawn a new one.

            This setup assumes that this NetworkComponent will ONLY EVER be accessed from a single-threaded
            context. If that situation no longer applies, we'll have to use some mutexes :(
        */

        public NetPeer Connection
        {
            get { return connection_; }
            set
            {
                /*
                    If we're assigning to ourselves, don't do anything. This avoids unecessary nasty thread start/stop times
                */
                if (value == connection_)
                {
                    return;
                }

                try
                {
                    ConnectionListener.Abort();
                }
                catch (NullReferenceException e)
                {
                    // This handles the startup case
                    LOG.Info(
                        "Caught a NullReferenceException while attempting to abort a connection listener. First time assignment?",
                        e);
                }
                catch (Exception e)
                {
                    LOG.Error("Caught an exception while attempting to abort a connection listener", e);
                }

                connection_ = value;

                ConnectionListener = new Thread(ReadFromConnection);
                ConnectionListener.Start();
            }
        }

        protected NetworkComponent(DxGame game)
            : base(game)
        {
            MessageQueue = new ConcurrentQueue<NetIncomingMessage>();
        }

        public virtual NetworkComponent WithConnection(NetPeer connection)
        {
            GenericUtils.CheckNullOrDefault(connection, "Cannot create a NetworkComponent with a null/default NetPeer");
            Connection = connection;
            return this;
        }

        public virtual NetworkComponent WithConfiguration(NetPeerConfiguration config)
        {
            GenericUtils.CheckNull(config, "Cannot create a NetworkComponent with a null NetPeerConfiguration");
            Connection = new NetPeer(config);
            return this;
        }

        protected static T ConvertMessageType<T>(NetworkMessage message) where T : class
        {
            return GenericUtils.CheckedCast<T>(message, LOG,
                String.Format("Received message expecting type {0}, but was unable to dynamic cast", typeof(T)));
        }

        // We need to know the GameTime in order to Receive Data
        public void ReceiveData(GameTime gameTime)
        {
            int maxMessages = MessageQueue.Count;
            for (int i = 0; i < maxMessages; ++i)
            {
                NetIncomingMessage incomingMessage = null;
                bool couldDequeue = MessageQueue.TryDequeue(out incomingMessage);
                if (!couldDequeue)
                {
                    // If we couldn't dequeue anything, hard-bail
                    LOG.Info(
                        String.Format("Expected {0} messages, but only received {1} before we could not dequeue any.",
                            maxMessages, i));
                    return;
                }

                if (incomingMessage == null)
                {
                    LOG.Info(String.Format("Found a null message inside the MessageQueue. This shouldn't happen."));
                    continue;
                }

                RouteDataOnMessageType(incomingMessage, gameTime);
            }
        }

        public abstract void EstablishConnection();

        public abstract void RouteDataOnMessageType(NetIncomingMessage message, GameTime gameTime);

        // ...and also to send data
        public abstract void SendData(GameTime gameTime);

        public abstract void Shutdown();
    }
}