using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Map;
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
            PLATFORM_COLLISIONS =
                new ReadOnlyDictionary<PlatformType, ReadOnlyCollection<CollidableDirection>>(
                    new Dictionary<PlatformType, ReadOnlyCollection<CollidableDirection>>
                    {
                        {
                            PlatformType.Block,
                            new ReadOnlyCollection<CollidableDirection>(
                                Enum.GetValues(typeof (CollidableDirection))
                                    .ToEnumerable<CollidableDirection>()
                                    .ToList())
                        },
                        {
                            PlatformType.Platform,
                            new ReadOnlyCollection<CollidableDirection>(
                                new List<CollidableDirection>(new[] {CollidableDirection.Up}))
                        }
                    });

        [DataMember]
        public PlatformType Type { get; set; }

        [DataMember]
        public DxRectangle BoundingBox { get; set; }

        [IgnoreDataMember]
        public IEnumerable<CollidableDirection> CollidableDirections => PLATFORM_COLLISIONS[Type];

        /* Only necessary for JSON Serialization */
        private Platform()
        {
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

        public override bool Equals(object other)
        {
            var platform = other as Platform;
            if (ReferenceEquals(platform, null))
            {
                return false;
            }

            return Type == platform.Type && BoundingBox == platform.BoundingBox;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Type, BoundingBox).GetHashCode();
        }

        public override string ToString()
        {
            return BoundingBox.ToString();
        }
    }
}