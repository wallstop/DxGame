using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Map
{
    public enum PlatformType
    {
        Block,
        Platform
    }

    [Serializable]
    [DataContract]
    public class Platform
    {
        public static readonly ReadOnlyDictionary<PlatformType, ReadOnlyCollection<CollidableDirection>>
            PlatformCollisions =
                new ReadOnlyDictionary<PlatformType, ReadOnlyCollection<CollidableDirection>>(
                    new Dictionary<PlatformType, ReadOnlyCollection<CollidableDirection>>
                    {
                        {
                            PlatformType.Block,
                            new ReadOnlyCollection<CollidableDirection>(
                                Enum.GetValues(typeof(CollidableDirection)).ToEnumerable<CollidableDirection>().ToList())
                        },
                        {
                            PlatformType.Platform,
                            new ReadOnlyCollection<CollidableDirection>(
                                new List<CollidableDirection>(new[] {CollidableDirection.Up}))
                        }
                    });

        /* Only necessary for JSON Serialization */

        private Platform() {}

        public Platform(PlatformType platformType, DxRectangle boundingBox)
        {
            Type = platformType;
            BoundingBox = boundingBox;
        }

        public Platform(Platform copy)
        {
            Validate.IsNotNull(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            BoundingBox = copy.BoundingBox;
            Type = copy.Type;
        }

        public Platform(DxRectangle boundingBox, PlatformType type = PlatformType.Block)
        {
            BoundingBox = boundingBox;
            Type = type;
        }

        [DataMember]
        public PlatformType Type { get; }

        [DataMember]
        public DxRectangle BoundingBox { get; }

        [IgnoreDataMember]
        public IEnumerable<CollidableDirection> CollidableDirections => PlatformCollisions[Type];

        public override bool Equals(object other)
        {
            var platform = other as Platform;
            if(!ReferenceEquals(platform, null))
            {
                return Type == platform.Type && BoundingBox == platform.BoundingBox;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Type, BoundingBox);
        }

        public override string ToString()
        {
            return BoundingBox.ToString();
        }
    }
}