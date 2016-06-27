using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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
            Validate.Hard.IsTrue(0 <= x);
            Validate.Hard.IsTrue(0 <= y);
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(X, Y);
        }

        public static bool operator ==(TilePosition lhs, TilePosition rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static bool operator !=(TilePosition lhs, TilePosition rhs)
        {
            return lhs.X != rhs.X || lhs.Y != rhs.Y;
        }

        public override bool Equals(object other)
        {
            return other is TilePosition && this == (TilePosition) other;
        }

        public override string ToString()
        {
            string jsonValue = this.ToJson();
            return jsonValue;
        }
    }
}
