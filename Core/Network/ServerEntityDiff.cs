using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Network
{
    [DataContract]
    [Serializable]
    public class ServerEntityDiff : NetworkMessage
    {
        [DataMember]
        public List<object> MissingGameElements { get; } = new List<object>();

        [DataMember]
        public DxGameTime GameTime { get; }

        public ServerEntityDiff()
        {
            MessageType = MessageType.ServerDataDiff;
            GameTime = DxGame.Instance.CurrentTime;
        }

        public ServerEntityDiff(ServerEntityTracker entityTracker) : this()
        {
            Validate.IsNotNull(entityTracker, StringUtils.GetFormattedNullOrDefaultMessage(this, entityTracker));
            MissingGameElements.AddRange(entityTracker.Entities);
        }
    }
}