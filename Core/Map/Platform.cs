using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Map
{
    public enum CollidableDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    [Serializable]
    [DataContract]
    public class Platform
    {
        [DataMember]
        public DxRectangle BoundingBox { get; set; }

        [DataMember]
        public HashSet<CollidableDirection> CollidableDirections { get; private set; } =
            new HashSet<CollidableDirection>();
    }
}