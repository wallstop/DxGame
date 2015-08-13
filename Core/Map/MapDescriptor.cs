using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapDescriptor
    {
        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public List<Platform> Platforms { get; private set; } = new List<Platform>();

        [DataMember]
        public DxRectangle Size { get; set; }
    }
}