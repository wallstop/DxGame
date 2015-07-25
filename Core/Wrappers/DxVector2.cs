using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Wrappers
{
    [Serializable]
    [DataContract]
    public struct DxVector2 : IEquatable<DxVector2>, IEquatable<Vector2>
    {
        [DataMember] public float X;
        [DataMember] public float Y;

        public DxVector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public DxVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public DxVector2(float value)
        {
            X = value;
            Y = value;
        }

        public bool Equals(DxVector2 other)
        {
            return this == other;
        }

        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public static DxVector2 operator +(DxVector2 lhs, DxVector2 rhs)
        {
            lhs.X += rhs.X;
            lhs.Y += rhs.Y;
            return lhs;
        }

        public static DxVector2 operator -(DxVector2 lhs, DxVector2 rhs)
        {
            lhs.X -= rhs.X;
            lhs.Y -= rhs.Y;
            return lhs;
        }

        public static DxVector2 operator *(DxVector2 lhs, DxVector2 rhs)
        {
            lhs.X *= rhs.X;
            lhs.Y *= rhs.Y;
            return lhs;
        }

        public static DxVector2 operator *(DxVector2 lhs, float scaleFactor)
        {
            lhs.X *= scaleFactor;
            lhs.Y *= scaleFactor;
            return lhs;
        }

        public static DxVector2 operator *(DxVector2 lhs, double scaleFactor)
        {
            return lhs * (float) scaleFactor;
        }

        public static DxVector2 operator /(DxVector2 lhs, DxVector2 rhs)
        {
            lhs.X /= rhs.X;
            lhs.Y /= rhs.Y;
            return lhs;
        }

        public static DxVector2 operator /(DxVector2 lhs, float divisor)
        {
            lhs.X /= divisor;
            lhs.Y /= divisor;
            return lhs;
        }

        public static bool operator ==(DxVector2 lhs, DxVector2 rhs)
        {
            return lhs.X.Equals(rhs.X) && lhs.Y.Equals(rhs.Y);
        }

        public static bool operator !=(DxVector2 lhs, DxVector2 rhs)
        {
            return !lhs.X.Equals(rhs.X) || !lhs.Y.Equals(rhs.Y);
        }

        public override bool Equals(object other)
        {
            if (other is DxVector2)
            {
                return Equals((DxVector2) other);
            }
            if (other is Vector2)
            {
                return Equals((Vector2) other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public override string ToString()
        {
            return "{X:" + X + " Y: " + Y + "}";
        }
    }
}