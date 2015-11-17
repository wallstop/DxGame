using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;

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
        public DxRectangle Size { get; set; }

        [DataMember]
        public List<MapTile> Platforms { get; set; } = new List<MapTile>();
    }
}