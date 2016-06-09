using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Lidgren.Network;
using NLog;

namespace DxCore.Core.Network
{
    /*
        TODO: Enumerate these as need be. These represent the individual "kinds" of message that
        can be received over the network and will generally have classes associated with them.
     */

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

        protected NetworkMessage()
        {
            Id = new UniqueId();
        }

        [DataMember]
        public UniqueId Id { get; private set; }

        [DataMember]
        public DxGameTime TimeStamp { get; private set; } = DxGame.Instance.CurrentTime;

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

        public NetOutgoingMessage ToNetOutgoingMessage(NetPeer sender)
        {
            Validate.IsNotNull(sender,
                $"Cannot create a NetOutgoingMessage from {this}, the provided sender connection is null!");
            var message = sender.CreateMessage();
            byte[] byteStream = Serializer<NetworkMessage>.BinarySerialize(this);
            message.Write(byteStream);
            return message;
        }
    }
}