using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using WallNetCore.Validate;

namespace DxCore.Core.Map
{
    /*
        TODO: Make more generic? Would it be a good idea to have int-based Vector2s?
    */

    /**
        <summary>
            Represents positions of tiles within the Map (think 2D grid)
        </summary>
        <note>
            X and Y should never be negative - they should be indexes into a 2D array
        </note>
     */

    [Serializable]
    [DataContract]
    public struct TilePosition
    {
        public static readonly int InvalidIndex = -1;

        [DataMember]
        public int X { get; private set; }

        [DataMember]
        public int Y { get; private set; }

        public TilePosition(int x, int y)
        {
            Validate.Hard.IsTrue(ValidTileCoordinates(x, y));
            X = x;
            Y = y;
        }

        public static bool ValidTileCoordinates(int x, int y)
        {
            return (0 <= x) && (0 <= y);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(X, Y);
        }

        public TilePosition? Neighbor(Direction direction)
        {
            switch(direction)
            {
                case Direction.East:
                {
                    return new TilePosition(X + 1, Y);
                }
                case Direction.North:
                {
                    if(Y == 0)
                    {
                        return null;
                    }
                    return new TilePosition(X, Y - 1);
                }
                case Direction.South:
                {
                    return new TilePosition(X, Y + 1);
                }
                case Direction.West:
                {
                    if(X == 0)
                    {
                        return null;
                    }
                    return new TilePosition(X - 1, Y);
                }
                default:
                {
                    throw new InvalidEnumArgumentException(
                        $"No neighbor mapping found for {typeof(Direction)} {direction}");
                }
            }
        }

        public static bool operator ==(TilePosition lhs, TilePosition rhs)
        {
            return (lhs.X == rhs.X) && (lhs.Y == rhs.Y);
        }

        public static bool operator !=(TilePosition lhs, TilePosition rhs)
        {
            return (lhs.X != rhs.X) || (lhs.Y != rhs.Y);
        }

        public override bool Equals(object other)
        {
            return other is TilePosition && (this == (TilePosition) other);
        }

        public override string ToString()
        {
            string jsonValue = this.ToJson();
            return jsonValue;
        }

        /* We need a public static Parse method if we want these to be dictionary keys & JSON serialized */
        public static TilePosition Parse(string toParse) => SerializerExtensions.Parse<TilePosition>(toParse);
    }
}