using System;
using System.Diagnostics;
using DXGame.Core.Utils;
using log4net;
using Lidgren.Network;

namespace DXGame.Core.Network
{
    /*
        Base view of our NetIncomingMessage so that we can tell what kind of class it actually is.

        These classes serve as a sort of "ORM"-ey style thing to generate and parse NetIncomingMessages
        and NetOutgoingMessages.
    */

    public class NetworkMessage
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkMessage));

        public MessageType MessageType { get; set; }

        public static NetworkMessage FromNetIncomingMessage(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot create a NetworkMessage object from a null NetIncomingMessage!");
            var typeString = message.ReadString();
            try
            {
                var networkMessage = NetworkUtils.ReadTypeFrom<NetworkMessage>(message);
                networkMessage.MessageType = (MessageType) message.ReadUInt32();
                networkMessage.LoadFromNetIncomingMessage(message);
                return networkMessage;
            }
            catch (Exception e)
            {
                // TODO: Log metrics on this
                var logMessage =
                    String.Format("Could not create a Network Message for type {0}, something went horribly wrong.",
                        typeString);
                LOG.Error(logMessage, e);
                Debug.Assert(false, logMessage);
                return new NetworkMessage();
            }
        }

        /*
            Override this with whatever custom behavior you want
        */

        public virtual void LoadFromNetIncomingMessage(NetIncomingMessage message)
        {
            throw new NotImplementedException(String.Format(
                "LoadFromNetIncomingMessage needs to be implemented for {0}", GetType()));
        }

        /*
            Override this with whatever custom behavior you want
        */

        public virtual NetOutgoingMessage WriteToNetOutgoingMessage(NetOutgoingMessage message)
        {
            throw new NotImplementedException(String.Format(
                "WriteToNetOutgoingMessage needs to be implemented for {0}", GetType()));
        }

        public NetOutgoingMessage ToNetOutgoingMessage(NetPeer connection)
        {
            GenericUtils.CheckNull(connection,
                String.Format(
                    "Cannot create a NetOutgoingMessage from {0}, the connection provided connection is null!", this));

            /*
                We always rely on having the type (as a string) be the first thing in a message, so 
                let's go ahead and make it an invariant."
            */
            var message = connection.CreateMessage();
            NetworkUtils.WriteTypeTo(this, message);
            message.Write((uint)MessageType);
            return WriteToNetOutgoingMessage(message);
        }
    }
}