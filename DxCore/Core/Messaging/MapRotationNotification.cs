using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class MapRotationNotification : Message
    {
        [DataMember]
        public Map.Map Map { get; private set; }

        public MapRotationNotification(Map.Map map)
        {
            Validate.Hard.IsNotNull(map);
            Map = map;
        }
    }
}
