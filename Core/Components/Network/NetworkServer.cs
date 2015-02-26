using System;
using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Network;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;

namespace DXGame.Core.Components.Network
{
    public class NetworkServer : NetworkComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkServer));

        public NetServer ServerConnection
        {
            get { return Connection as NetServer; }
        }

        public Dictionary<NetConnection, FrameModel> ClientFrameStates { get; set; }

        public NetworkServer(DxGame game)
            : base(game)
        {
        }

        protected override void EstablishConnection()
        {
            throw new NotImplementedException();
        }

        public override void ReceiveData()
        {
            GenericUtils.CheckNullOrDefault(ServerConnection, "Cannot receive data from a null Server Connection");
            /*
                We should probably do a for(int i = 0; i < numClients; ++i) { ServerConnection.ReadMessage(); } 
                Style of thing here... but then we run into the issue of out of order messages / delayed messages...
                
                This will require some thought to get it to work properly
            */
            var incomingMessage = ServerConnection.ReadMessage();
            if (incomingMessage == null)
            {
                // If we don't have a message, we're done here.
                return;
            }

            ProcessData(incomingMessage);
        }

        public override void SendData()
        {
            throw new NotImplementedException();
        }

        protected void ProcessData(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot process server data on a null message!");
            var networkMessage = NetworkMessage.FromNetIncomingMessage(message);
            GenericUtils.CheckNull(networkMessage,
                "Could not properly format a NetworkMessage from the NetIncomingMessage");
            switch (networkMessage.MessageType)
            {
            case MessageType.CLIENT_DATA_DIFF:
                HandleClientDataDiff(networkMessage);
                break;
            case MessageType.CLIENT_CONNECTION_REQUEST:
                HandleClientConnectionRequest(networkMessage);
                break;
            case MessageType.SERVER_DATA_DIFF:
                HandleServerDataDiff(networkMessage);
                break;
            case MessageType.SERVER_DATA_KEYFRAME:
                HandleServerDataKeyframe(networkMessage);
                break;
            }
        }

        protected void HandleClientDataDiff(NetworkMessage message)
        {
            var clientDataDiffMessage = message as GameStateDiff;
            if (clientDataDiffMessage == null)
            {
                // TODO: Log metrics on this
                var logMessage = String.Format(
                    "Received ClientDataDiff message type, but was unable to cast message as a GameStateDiff ({0})",
                    message);
                LOG.Error(logMessage);
                Debug.Assert(false, logMessage);
            }

            // TODO
            throw new NotImplementedException();
        }

        protected void HandleClientConnectionRequest(NetworkMessage message)
        {
            var clientConnectionRequest = message as ClientConnectionRequest;
            if (clientConnectionRequest == null)
            {
                // TODO: Log metrics on this
                var logMessage = String.Format(
                    "Received ClientConnectionRequest message type, but was unable to cast message as ClientConnectionRequest ({0})",
                    message);
                LOG.Error(logMessage);
                Debug.Assert(false, logMessage);
            }

            // TODO
            var clientConnection = clientConnectionRequest

            throw new NotImplementedException();
        }

        protected void HandleServerDataDiff(NetworkMessage message)
        {
            var logMessage = String.Format("Received a Server Data Diff {0} from a client. This should not happen",
                message);
            LOG.Error(logMessage);
            // Might want to raise an exception for release mode
            Debug.Assert(false, logMessage);
        }

        protected void HandleServerDataKeyframe(NetworkMessage message)
        {
            var logMessage = String.Format("Received a Server Data Keyframe {0} from a client. This should not happen",
                message);
            LOG.Error(logMessage);
            // Might want to raise an exception for release mode
            Debug.Assert(false, logMessage);
        }
    }
}