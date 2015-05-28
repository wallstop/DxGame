using System;
using System.Diagnostics;
using System.Runtime.Serialization;
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

    [Serializable]
    [DataContract]
    public class NetworkMessage
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NetworkMessage));

        [DataMember]
        public MessageType MessageType { get; set; }

        public static NetworkMessage FromNetIncomingMessage(NetIncomingMessage message)
        {
            Validate.IsNotNull(message, $"Cannot create a {typeof (NetworkMessage)} from a null NetIncomingMessage!");
            var typeString = message.ReadString();
            try
            {
                return Serializer<NetworkMessage>.BinaryDeserialize(message.PeekDataBuffer());
            }
            catch (Exception e)
            {
                // TODO: Log metrics on this
                var logMessage =
                    $"Could not create a {typeof(NetworkMessage)} for type {typeString}, something went horribly wrong.";
                LOG.Error(logMessage, e);
                Debug.Assert(false, logMessage);
                return new NetworkMessage();
            }
        }

        public NetOutgoingMessage ToNetOutgoingMessage(NetPeer connection)
        {
            Validate.IsNotNull(connection,
                $"Cannot create a NetOutgoingMessage from {this}, the connection provided connection is null!");

            /*
                We always rely on having the type (as a string) be the first thing in a message, so 
                let's go ahead and make it an invariant.
            */
            var message = connection.CreateMessage();
            byte[] byteStream = Serializer<NetworkMessage>.BinarySerialize(this);
            message.Write(byteStream);
            return message;
        }
    }
}