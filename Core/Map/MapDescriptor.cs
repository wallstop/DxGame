using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapDescriptor : JsonPersistable<MapDescriptor>
    {
        public static string MapExtension => ".mdtr";
        [IgnoreDataMember]
        public override string Extension => MapExtension;
        [IgnoreDataMember]
        public override MapDescriptor Item => this;

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public List<Platform> Platforms { get; set; } = new List<Platform>();

        [DataMember]
        public DxRectangle Size { get; set; }

        [DataMember]
        public float Scale { get; set; } = 1.0f;
    }
}