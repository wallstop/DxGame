using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using Lidgren.Network;
using NLog;

namespace DXGame.Core.Network
{
    /*
        TODO: Enumerate these as need be. These represent the individual "kinds" of message that
        can be received over the network and will generally have classes associated with them.
     */

    public enum MessageType
    {
        Invalid,
        Acknowledgement,
        ClientConnectionRequest, // Client connect to server
        ClientDataDiff, // Client info of how it thought a frame went
        ClientKeyFrame, // Client full-state dump
        ServerDataDiff, // Server info of the diff between it's last update and "now"
        ServerDataKeyFrame // Server full-state dump
    }

    /*
        Base view of our NetIncomingMessage so that we can tell what kind of class it actually is.

        These classes serve as a sort of "ORM"-ey style thing to generate and parse NetIncomingMessages
        and NetOutgoingMessages.
    */

    [Serializable]
    [DataContract]
    public class NetworkMessage : IIdentifiable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public MessageType MessageType { get; set; }

        protected NetworkMessage()
        {
            Id = new UniqueId();
        }

        public UniqueId Id { get; }

        public static NetworkMessage FromNetIncomingMessage(NetIncomingMessage message)
        {
            Validate.IsNotNull(message,
                $"Cannot create a {typeof(NetworkMessage)} from a null {typeof(NetIncomingMessage)}!");
            try
            {
                return Serializer<NetworkMessage>.BinaryDeserialize(message.PeekDataBuffer());
            }
            catch(Exception e)
            {
                // TODO: Log metrics on this
                var logMessage =
                    $"Could not create a {typeof(NetworkMessage)} for unknown type, something went horribly wrong.";
                LOG.Error(e, logMessage);
                throw;
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