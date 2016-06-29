using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Primitives
{
    [Serializable]
    [DataContract]
    public struct DxCircle : IEquatable<DxCircle>, IShape
    {
        [DataMember]
        public DxVector2 Center { get; }

        [DataMember]
        public float Radius { get; }

        [IgnoreDataMember]
        public DxRectangle Bounds => new DxRectangle(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

        public DxCircle(DxVector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Equals(DxCircle other)
        {
            return Center.Equals(other.Center) && Radius.FuzzyCompare(other.Radius) == 0;
        }

        public bool Contains(DxVector2 point)
        {
            return (Center - point).MagnitudeSquared < Radius * Radius;
        }

        public bool Intersects(DxRectangle rectangle)
        {
            float closestX = MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right);
            /* 
                We need to invert Top & Bottom here - in XNA land, negative values are 
                "less than" positive values, but here, we're talking about strict numerical relationships 
            */
            float closestY = MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom);

            DxVector2 closestPoint = new DxVector2(closestX, closestY);
            return Contains(closestPoint);
        }

        public static DxCircle operator *(DxCircle circle, float scalar)
        {
            return new DxCircle(circle.Center, circle.Radius * scalar);
        }

        public static DxCircle operator /(DxCircle circle, float scalar)
        {
            return circle * (1.0f / scalar);
        }

        public override string ToString()
        {
            return $"{{ Center: {Center}, Radius: {Radius:N2} }}";
        }

        public static bool operator !=(DxCircle lhs, DxCircle rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(DxCircle lhs, DxCircle rhs)
        {
            return lhs.Center == rhs.Center && lhs.Radius.FuzzyCompare(rhs.Radius) == 0;
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }
            return other is DxCircle && Equals((DxCircle) other);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Center, Radius);
        }
    }
}