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

        public TimeSpan TimeStamp { get; set; }
        public MessageType MessageType { get; set; }

        public static NetworkMessage FromNetIncomingMessage(NetIncomingMessage message)
        {
            GenericUtils.CheckNull(message, "Cannot create a NetworkMessage object from a null NetIncomingMessage!");
            var typeString = message.ReadString();
            try
            {
                var type = Type.GetType(typeString, true);
                var networkMessage = (NetworkMessage) Activator.CreateInstance(type);
                networkMessage.LoadFromNetIncomingMessage(message);
                return networkMessage;
            }
            catch (Exception e)
            {
                // TODO: Log metrics on this
                var logMessage =
                    String.Format("Could not create a Network Message for type {0}, something went horribly wrong.",
                        typeString);
                LOG.Error(logMessage);
                Debug.Assert(false, logMessage);
                return new NetworkMessage();
            }
        }

        /*
            Default behavior is to read in :
                -All properties

            Override this only if you must.
        */

        public virtual void LoadFromNetIncomingMessage(NetIncomingMessage message)
        {
            message.ReadAllProperties(this);
        }

        /*
            Default behavior is to write out:
                -Type (string)
                -All Properties

            You should have a *VERY GOOD REASON* for over-riding this. If you do, make sure the LoadFromNetIncomingMessage
            and ToNetOutgoingMessage line up. 

            Additionally, we ALWAYS need to write the type first. Always always always, no matter how stupid your
            implementation of this. This is a data contract that we hold with the NetworkComponents.
        */

        public virtual NetOutgoingMessage ToNetOutgoingMessage(NetPeer connection)
        {
            GenericUtils.CheckNull(connection,
                String.Format(
                    "Cannot create a NetOutgoingMessage from {0}, the connection provided connection is null!", this));

            var type = GetType();

            var message = connection.CreateMessage();
            message.Write(type.ToString());
            message.WriteAllProperties(this);
            return connection.CreateMessage();
        }
    }
}