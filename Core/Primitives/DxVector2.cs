using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Primitives
{
    public enum Orientation
    {
        Colinear,
        Clockwise,
        CounterClockwise
    }

    [Serializable]
    [DataContract]
    public struct DxVector2 : IEquatable<DxVector2>, IEquatable<Vector2>, IComparable<DxVector2>
    {
        [DataMember] public float X;
        [DataMember] public float Y;
        public float MagnitudeSquared => X * X + Y * Y;
        public float Magnitude => (float) Math.Sqrt(MagnitudeSquared);
        public DxRadian Radian => new DxRadian(this);
        public DxDegree Degree => new DxDegree(this);
        public static DxVector2 EmptyVector => new DxVector2();

        public DxVector2 UnitVector
        {
            get
            {
                var magnitude = Magnitude;
                return new DxVector2(X / magnitude, Y / magnitude);
            }
        }

        public DxVector2(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public DxVector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public DxVector2(DxVector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public DxVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public DxVector2(double x, double y) : this((float) x, (float) y) {}

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

        /**
            <summary> Dot product (https://en.wikipedia.org/wiki/Dot_product) </summary>
        */

        public float Dot(DxVector2 other)
        {
            return X * other.X + Y * other.Y;
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

        public static Orientation Orientation(DxVector2 point1, DxVector2 point2, DxVector2 point3)
        {
            int orientation =
                (int) ((point2.Y - point1.Y) * (point3.X - point2.X) - (point2.X - point1.X) * (point3.Y - point2.Y));
            if(orientation == 0)
            {
                return Primitives.Orientation.Colinear;
            }
            return orientation > 0 ? Primitives.Orientation.Clockwise : Primitives.Orientation.CounterClockwise;
        }

        public static double DistanceBetween(DxVector2 lhs, DxVector2 rhs)
        {
            return Math.Sqrt(DistanceBetweenSquared(lhs, rhs));
        }

        public static double DistanceBetweenSquared(DxVector2 lhs, DxVector2 rhs)
        {
            return Math.Pow(rhs.X - lhs.X, 2) + Math.Pow(rhs.Y - lhs.Y, 2);
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
            if(other is DxVector2)
            {
                return Equals((DxVector2) other);
            }
            if(other is Vector2)
            {
                return Equals((Vector2) other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(X, Y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public override string ToString()
        {
            return $"{{ X:{X:N2}, Y:{Y:N2} }}";
        }

        public int CompareTo(DxVector2 other)
        {
            int magnitudeCompare = MagnitudeSquared.CompareTo(other.MagnitudeSquared);
            if(magnitudeCompare != 0)
            {
                return magnitudeCompare;
            }
            int xCompare = X.CompareTo(other.X);
            if(xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }
    }
}