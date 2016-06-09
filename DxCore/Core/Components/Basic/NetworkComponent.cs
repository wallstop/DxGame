using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DxCore.Core.Network;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Lidgren.Network;
using NLog;

namespace DxCore.Core.Components.Basic
{
    /*
        Basic networked component. In general, there should really only be two types of these - Client & Server
    */

    public abstract class NetworkComponent : Component
    {
        protected const int REQUIRED_MESSAGE_CHANNEL = 0;
        protected const int LERP_DATA_CHANNEL = 1;
        protected const int TIME_SYNCHRONIZATION_CHANNEL = 2;
        protected const int PLAYER_INPUT_CHANNEL = 3;
        protected const int CLIENT_SPECIFIC_UPDATES = 4;

        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private NetPeer connection_;
        /*
            Make sure you know what you're doing when you get/set this. 
            This Connection listener should only be set via Connection setter and never gotten
        */
        protected Thread ConnectionListener { get; set; }
        protected ConcurrentQueue<NetIncomingMessage> MessageQueue { get; set; }

        private static readonly TimeSpan NETWORK_POLL_DELAY = TimeSpan.FromMilliseconds(1.0 / 10);

        public abstract TimeSpan TickRate { get; }

        protected Stopwatch TransmissionClock { get; }

        private TimeSpan lastTicked_;

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
                    LOG.Error(e, "Caught an exception while attempting to abort a connection listener");
                }

                connection_ = value;

                ConnectionListener = new Thread(ReadFromConnection);
                ConnectionListener.Start();
            }
        }

        protected Dictionary<Type, Action<NetworkMessage, NetConnection>> networkMessageHandlers_;

        protected NetworkComponent()
        {
            MessageQueue = new ConcurrentQueue<NetIncomingMessage>();
            networkMessageHandlers_ = new Dictionary<Type, Action<NetworkMessage, NetConnection>>();
            TransmissionClock = Stopwatch.StartNew();
        }

        public override void Initialize()
        {
            InitializeNetworkMessageListeners();
            base.Initialize();
        }

        protected void ReadFromConnection()
        {
            try
            {
                while (true)
                {
                    NetIncomingMessage message;
                    while ((message = Connection.ReadMessage()) != null)
                    {
                        MessageQueue.Enqueue(message);
                    }
                    Thread.Sleep(NETWORK_POLL_DELAY);
                }
            }
            catch (ThreadInterruptedException e)
            {
                LOG.Info($"Shutting down reader for Connection {Connection}", e);
            }
        }

        public virtual NetworkComponent WithConnection(NetPeer connection)
        {
            Validate.IsNotNullOrDefault(connection, "Cannot create a NetworkComponent with a null/default NetPeer");
            Connection = connection;
            return this;
        }

        public override bool ShouldSerialize => false;

        public abstract NetworkComponent WithConfiguration(NetPeerConfiguration config);

        protected static T ConvertMessageType<T>(NetworkMessage message) where T : class
        {
            return GenericUtils.CheckedCast<T>(message,
                $"Received message expecting type {typeof (T)}, but was unable to dynamic cast");
        }

        // We need to know the GameTime in order to Receive Data
        public void ReceiveData(DxGameTime gameTime)
        {
            int maxMessages = MessageQueue.Count;
            for (int i = 0; i < maxMessages; ++i)
            {
                NetIncomingMessage incomingMessage;
                bool couldDequeue = MessageQueue.TryDequeue(out incomingMessage);
                if (!couldDequeue)
                {
                    // If we couldn't dequeue anything, hard-bail
                    LOG.Info(
                        $"Expected {maxMessages} messages, but only received {i} before we could not dequeue any.");
                    return;
                }

                if (incomingMessage == null)
                {
                    LOG.Info("Found a null message inside the MessageQueue. This shouldn't happen.");
                    continue;
                }

                RouteDataOnMessageType(incomingMessage, gameTime);
            }

            PostReceiveData(gameTime);
        }

        protected virtual void PostReceiveData(DxGameTime gameTime)
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
            catch(Exception e)
            {
                LOG.Error(e, "Caught unexpected exception while attempting to process network data message");
            }
            finally
            {
                Validate.IsNotNull(networkMessage,
                    $"Could not properly format a NetworkMessage from NetIncomingMessage {message}");
            }

            Type trueMessageType = networkMessage.GetType();
            Action<NetworkMessage, NetConnection> demarshaller;
            if(networkMessageHandlers_.TryGetValue(trueMessageType, out demarshaller))
            {
                demarshaller.Invoke(networkMessage, message.SenderConnection);
            }
            else
            {
                LOG.Error($"Received an invalid message ({message}) from a client, ignoring");
            }
        }

        public abstract void EstablishConnection();
        public abstract void RouteDataOnMessageType(NetIncomingMessage message, DxGameTime gameTime);
        // ...and also to send data
        public void SendData(DxGameTime gameTime)
        {
            TimeSpan currentTick = TransmissionClock.Elapsed;
            if(currentTick <= lastTicked_ + TickRate)
            {
                return;
            }

            InternalSendData(gameTime);
            lastTicked_ = currentTick;
        }

        protected abstract void InternalSendData(DxGameTime gameTime);

        protected abstract void InitializeNetworkMessageListeners();

        public virtual void Shutdown()
        {
            ConnectionListener.Abort();
        }
    }
}