using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Map
{
    public enum PlatformType
    {
        Block,
        Ladder
    }

    [Serializable]
    [DataContract]
    public class Platform
    {
        [DataMember]
        public PlatformType Type { get; set; } = PlatformType.Block;

        [DataMember]
        public DxRectangle BoundingBox { get; set; }

        [DataMember]
        public List<CollidableDirection> CollidableDirections { get; private set; } =
            new List<CollidableDirection>();
    }
}