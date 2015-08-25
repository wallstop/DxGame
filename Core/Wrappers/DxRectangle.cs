using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Wrappers
{
    [Serializable]
    [DataContract]
    public struct DxRectangle : IEquatable<DxRectangle>, IEquatable<Rectangle>
    {
        private const float TOLERANCE = 0.000001f;
        [DataMember] public float Height;
        [DataMember] public float Width;
        [DataMember] public float X;
        [DataMember] public float Y;
        public float Area => Width * Height;
        public DxRectangle EmptyRectangle => new DxRectangle();
        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;
        public Point Center => new Point((int) ((X + Width) / 2), (int) ((Y + Height) / 2));
        /* 
            Cartesian quadrants of this rectangle:
            https://en.wikipedia.org/wiki/Quadrant_%28plane_geometry%29

            Where the quadrants are

            II  | I
            ____|___
                |
            III | IV
        */
        public DxRectangle QuadrantOne => new DxRectangle(X + (Width / 2), Y, Width / 2, Height / 2);
        public DxRectangle QuadrantTwo => new DxRectangle(X, Y, Width / 2, Height / 2);
        public DxRectangle QuadrantThree => new DxRectangle(X, Y + (Height / 2), Width / 2, Height / 2);
        public DxRectangle QuadrantFour => new DxRectangle(X + (Width / 2), Y + (Height / 2), Width / 2, Height / 2);

        public DxRectangle(Point upperLeftCorner, Point lowerRightCorner)
        {
            X = upperLeftCorner.X;
            Width = lowerRightCorner.X - upperLeftCorner.X;
            Y = upperLeftCorner.Y;
            Height = upperLeftCorner.Y - lowerRightCorner.Y;
        }

        public DxRectangle(Vector2 x, Vector2 y)
        {
            X = x.X;
            Width = x.Y;
            Y = y.X;
            Height = y.Y;
        }

        public DxRectangle(DxVector2 x, DxVector2 y)
        {
            X = x.X;
            Width = x.Y;
            Y = y.X;
            Height = y.Y;
        }

        public DxRectangle(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public DxRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Equals(DxRectangle rhs)
        {
            return this == rhs;
        }

        public bool Equals(Rectangle other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int) X, (int) Y, (int) Width, (int) Height);
        }

        public static DxRectangle FromRectangle(Rectangle rectangle)
        {
            return new DxRectangle(rectangle);
        }

        public Vector2 XY()
        {
            return new Vector2(X, Y);
        }

        public static bool operator ==(DxRectangle lhs, DxRectangle rhs)
        {
            return MathUtils.FuzzyCompare(lhs.X, rhs.X, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Y, rhs.Y, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Width, rhs.Width, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Height, rhs.Height, TOLERANCE) == 0;
        }

        public static bool operator !=(DxRectangle lhs, DxRectangle rhs)
        {
            return !(lhs == rhs);
        }

        public static DxRectangle operator *(DxRectangle lhs, float scalar)
        {
            return new DxRectangle(lhs.X * scalar, lhs.Y * scalar, lhs.Width * scalar, lhs.Height * scalar);
        }

        public static DxRectangle operator *(DxRectangle lhs, double scalar)
        {
            return new DxRectangle((float) (lhs.X * scalar), (float) (lhs.Y * scalar), (float) (lhs.Width * scalar),
                (float) (lhs.Height * scalar));
        }

        public static DxRectangle operator /(DxRectangle lhs, double scalar)
        {
            return lhs * (1.0 / scalar);
        }

        public static DxRectangle operator /(DxRectangle lhs, float scalar)
        {
            return lhs * (1.0 / scalar);
        }

        public bool Contains(float x, float y)
        {
            return X <= x && x < (X + Width) && Y <= y && y < (Y + Height);
        }

        public bool Contains(DxVector2 point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(Point point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Intersects(Rectangle rectangle)
        {
            return rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom && Top < rectangle.Bottom;
        }

        public bool Intersects(DxRectangle rectangle)
        {
            return rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom && Top < rectangle.Bottom;
        }

        public List<DxRectangle> Divide(float maxWidth, float maxHeight)
        {
            var numXSections = Width / maxWidth;
            var numYSections = Height / maxHeight;
            List<DxRectangle> dividedRegions = new List<DxRectangle>((int) Math.Max(1, numXSections * numYSections));
            for (int i = 0; i < numXSections; ++i)
            {
                float startX = X + maxWidth * i;
                float endX = X + Math.Min(maxWidth * (i + 1), Width);
                for (int j = 0; j < numYSections; ++j)
                {
                    float startY = Y + maxHeight * j;
                    float endY = Y + Math.Min(maxHeight * (j + 1), Height);
                    DxRectangle division = new DxRectangle(startX, startY, endX - startX, endY - startY);
                    dividedRegions.Add(division);
                }
            }
            return dividedRegions;
        }

        public static DxRectangle Intersect(DxRectangle lhs, DxRectangle rhs)
        {
            DxRectangle intersection;
            Intersect(lhs, rhs, out intersection);
            return intersection;
        }

        private static void Intersect(DxRectangle lhs, DxRectangle rhs, out DxRectangle output)
        {
            if (lhs.Intersects(rhs))
            {
                float widthDifference = Math.Min(lhs.X + lhs.Width, rhs.X + rhs.Width);
                float x = Math.Max(lhs.X, rhs.X);
                float heightDifference = Math.Min(lhs.Y + lhs.Height, rhs.Y + rhs.Height);
                float y = Math.Max(lhs.Y, rhs.Y);
                output = new DxRectangle(x, y, widthDifference - x, heightDifference - y);
            }
            else
            {
                output = new DxRectangle(0, 0, 0, 0);
            }
        }

        public static DxRectangle Intersect(DxRectangle lhs, Rectangle rhs)
        {
            DxRectangle intersection;
            Intersect(lhs, rhs, out intersection);
            return intersection;
        }

        private static void Intersect(DxRectangle lhs, Rectangle rhs, out DxRectangle output)
        {
            if (lhs.Intersects(rhs))
            {
                float widthDifference = Math.Min(lhs.X + lhs.Width, rhs.X + rhs.Width);
                float x = Math.Max(lhs.X, rhs.X);
                float heightDifference = Math.Min(lhs.Y + lhs.Height, rhs.Y + rhs.Height);
                float y = Math.Max(lhs.Y, rhs.Y);
                output = new DxRectangle(x, y, widthDifference - x, heightDifference - y);
            }
            else
            {
                output = new DxRectangle(0, 0, 0, 0);
            }
        }

        public override bool Equals(object rhs)
        {
            try
            {
                var rectangle = (DxRectangle) rhs;
                return Equals(rectangle);
            }
            catch (Exception)
            {
                // Suprress any class cast exceptions
            }

            return false;
        }

        public override string ToString()
        {
            return $"{{X:{X:N2} Y:{Y:N2} Width:{Width:N2} Height:{Height:N2}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                return hash;
            }
        }
    }
}