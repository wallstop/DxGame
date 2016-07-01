using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class Tile
    {
        [DataMember]
        public TileType Type { get; private set; }

        [DataMember]
        public string Asset { get; private set; }

        /* Only necessary for JSON Serialization */

        private Tile()
        {
            Type = TileType.None;
            Asset = "EmptyTile";
        }

        public Tile(Tile copy)
        {
            Validate.Hard.IsNotNull(copy, this.GetFormattedNullOrDefaultMessage(copy));
            Type = copy.Type;
            Asset = copy.Asset;
        }

        public Tile(TileType type, string asset)
        {
            Validate.Hard.IsNotNull(type, this.GetFormattedNullOrDefaultMessage(type));
            Validate.Hard.IsNotNullOrDefault(asset, this.GetFormattedNullOrDefaultMessage(nameof(asset)));
            Type = type;
            Asset = asset;
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
