using System;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DXGame.Core.Primitives;

namespace DxCore.Core.Messaging.Network
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
