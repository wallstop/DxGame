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
        public PlatformType Type { get; }

        [DataMember]
        public DxRectangle BoundingBox { get; }

        [DataMember]
        public List<CollidableDirection> CollidableDirections { get; } =
            new List<CollidableDirection>();

        public Platform(DxRectangle boundingBox, PlatformType type = PlatformType.Block)
        {
            BoundingBox = boundingBox;
            Type = type;
        }

        public override bool Equals(object other)
        {
            var platform = other as Platform;
            if (ReferenceEquals(platform, null))
            {
                return false;
            }

            return Type == platform.Type && BoundingBox == platform.BoundingBox &&
                   CollidableDirections == platform.CollidableDirections;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Type, BoundingBox, CollidableDirections).GetHashCode();
        }

        public override string ToString()
        {
            return BoundingBox.ToString();
        }
    }
}