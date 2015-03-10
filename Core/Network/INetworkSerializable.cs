using System;
using Lidgren.Network;

namespace DXGame.Core.Network
{
    public interface INetworkSerializable
    {
        void SerializeTo(NetOutgoingMessage message);
        void DeserializeFrom(NetIncomingMessage messsage);
    }
}