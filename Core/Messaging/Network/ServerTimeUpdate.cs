using System;
using System.Runtime.Serialization;
using DXGame.Core.Network;
using DXGame.Core.Primitives;

namespace DXGame.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class ServerTimeUpdate : NetworkMessage
    {
        [DataMember]
        public TimeSpan ClientGameTime { get; private set; }

        [DataMember]
        public TimeSpan ServerGameTime { get; private set; }

        public ServerTimeUpdate(TimeSpan clientGameTime, TimeSpan serverGameTime)
        {
            ClientGameTime = clientGameTime;
            ServerGameTime = serverGameTime;
        }

        public ServerTimeUpdate(TimeSpan clientGameTime, DxGameTime serverGameTime)
            : this(clientGameTime, serverGameTime.TotalGameTime)
        {
        }
    }
}
