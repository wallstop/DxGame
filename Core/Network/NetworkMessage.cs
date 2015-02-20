using System;
using DXGame.Core.Utils;
using Lidgren.Network;

namespace DXGame.Core.Network
{
    /*
        Base view of our NetIncomingMessage so that we can tell what kind of class it actually is

    */
    public class NetworkMessage
    {
        public static MessageType MessageType(NetIncomingMessage message)
        {
                var currentPosition = message.Position;
                // We only want to read the head, make sure we don't screw up any in-progress reads
                message.Position = 0;
                var messageType = (MessageType) message.ReadByte();
                message.Position = currentPosition;
                return messageType;
        }
    }
}