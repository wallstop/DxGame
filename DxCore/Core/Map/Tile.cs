using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class Tile
    {
        public static readonly ReadOnlyDictionary<TileType, ReadOnlyCollection<CollidableDirection>>
            TILE_COLLISION_DIRECTIONS =
                new ReadOnlyDictionary<TileType, ReadOnlyCollection<CollidableDirection>>(
                    new Dictionary<TileType, ReadOnlyCollection<CollidableDirection>>
                    {
                        {
                            TileType.Block,
                            new ReadOnlyCollection<CollidableDirection>(
                                Enum.GetValues(typeof(CollidableDirection)).ToEnumerable<CollidableDirection>().ToList())
                        },
                        {
                            TileType.Platform,
                            new ReadOnlyCollection<CollidableDirection>(
                                new List<CollidableDirection>(new[] {CollidableDirection.Up}))
                        }
                    });

        [DataMember]
        public TileType Type { get; }

        [DataMember]
        public string Asset { get; }

        [IgnoreDataMember]
        public IEnumerable<CollidableDirection> CollidableDirections => TILE_COLLISION_DIRECTIONS[Type];

        /* Only necessary for JSON Serialization */

        private Tile()
        {
            Type = TileType.None;
            Asset = "EmptyTile";
        }

        public Tile(Tile copy)
        {
            Validate.IsNotNull(copy, this.GetFormattedNullOrDefaultMessage(copy));
            Type = copy.Type;
            Asset = copy.Asset;
        }

        public Tile(TileType type, string asset)
        {
            Validate.IsNotNull(type, this.GetFormattedNullOrDefaultMessage(type));
            Validate.IsNotNullOrDefault(asset, this.GetFormattedNullOrDefaultMessage(nameof(asset)));
            Type = type;
            Asset = asset;
        }

        public bool CollidesWith(DxVector2 velocity)
        {
            return (velocity.X > 0 && CollidableDirections.Contains(CollidableDirection.Left)) ||
                   (velocity.X < 0 && CollidableDirections.Contains(CollidableDirection.Right)) ||
                   (velocity.Y < 0 && CollidableDirections.Contains(CollidableDirection.Down)) ||
                   (velocity.Y > 0 && CollidableDirections.Contains(CollidableDirection.Up));
        }

        public override bool Equals(object other)
        {
            Tile tile = other as Tile;
            if(!ReferenceEquals(tile, null))
            {
                return Type.Equals(tile.Type) && Asset.Equals(tile.Asset);
            }
            return false;
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Type, Asset);
        }
    }
}
