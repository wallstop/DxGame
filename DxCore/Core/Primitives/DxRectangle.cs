using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Primitives
{
    [Serializable]
    [DataContract]
    public struct DxRectangle : IEquatable<DxRectangle>, IEquatable<Rectangle>
    {
        private const float Tolerance = 0.000001f;
        [DataMember] public float Height;
        [DataMember] public float Width;
        [DataMember] public float X;
        [DataMember] public float Y;
        public float Area => Width * Height;
        public static DxRectangle EmptyRectangle => new DxRectangle();

        public DxVector2 TopLeft => new DxVector2(Left, Top);
        public DxVector2 TopRight => new DxVector2(Right, Top);
        public DxVector2 BottomLeft => new DxVector2(Left, Bottom);
        public DxVector2 BottomRight => new DxVector2(Right, Bottom);

        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public Rectangle Rectangle => ToRectangle();

        public DxVector2 Dimensions => new DxVector2(Width, Height);

        public DxVector2 Center => new DxVector2(X + Width / 2, Y + Height / 2);
        /* 
            Cartesian quadrants of this rectangle:
            https://en.wikipedia.org/wiki/Quadrant_%28plane_geometry%29

            Where the quadrants are

            II  | I
            ____|___
                |
            III | IV
        */
        public DxRectangle QuadrantOne => new DxRectangle(X + Width / 2, Y, Width / 2, Height / 2);
        public DxRectangle QuadrantTwo => new DxRectangle(X, Y, Width / 2, Height / 2);
        public DxRectangle QuadrantThree => new DxRectangle(X, Y + Height / 2, Width / 2, Height / 2);
        public DxRectangle QuadrantFour => new DxRectangle(X + Width / 2, Y + Height / 2, Width / 2, Height / 2);

        public DxLineSegment LeftBorder => new DxLineSegment(X, Y, X, Y + Height);
        public DxLineSegment RightBorder => new DxLineSegment(X + Width, Y, X + Width, Y + Height);
        public DxLineSegment BottomBorder => new DxLineSegment(X, Y + Height, X + Width, Y + Height);
        public DxLineSegment TopBorder => new DxLineSegment(X, Y, X + Width, Y);

        private const int NumEdges = 4;

        public Dictionary<Direction, DxLineSegment> Edges
            =>
            new Dictionary<Direction, DxLineSegment>(NumEdges)
            {
                [Direction.East] = RightBorder,
                [Direction.West] = LeftBorder,
                [Direction.North] = TopBorder,
                [Direction.South] = BottomBorder
            };

        public DxVector2 Position => new DxVector2(X, Y);

        public List<DxLineSegment> Lines
        {
            get
            {
                List<DxLineSegment> lines = new List<DxLineSegment>(NumEdges)
                {
                    TopBorder,
                    BottomBorder,
                    LeftBorder,
                    RightBorder
                };
                return lines;
            }
        }

        // TODO: THIS SHIT AINT RIGHT
        public DxRectangle(DxVector2 upperLeftCorner, DxVector2 lowerRightCorner)
        {
            X = upperLeftCorner.X;
            Width = lowerRightCorner.X - upperLeftCorner.X;
            Y = upperLeftCorner.Y;
            Height = lowerRightCorner.Y - upperLeftCorner.Y;
        }

        public DxRectangle(DxVector2 upperLeftCorner, float width, float height)
        {
            X = upperLeftCorner.X;
            Width = width;
            Y = upperLeftCorner.Y;
            Height = height;
        }

        public DxRectangle(Point upperLeftCorner, Point lowerRightCorner)
            : this(upperLeftCorner.ToVector2(), lowerRightCorner.ToVector2()) {}

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

        public DxRectangle(double x, double y, double width, double height)
            : this((float) x, (float) y, (float) width, (float) height) {}

        public bool Equals(DxRectangle rhs)
        {
            return (X == rhs.X) && (Y == rhs.Y) && (Width == rhs.Width) && (Height == rhs.Height);
        }

        public bool Equals(Rectangle other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int) Math.Round(X), (int) Math.Round(Y), (int) Math.Round(Width),
                (int) Math.Round(Height));
        }

        public static implicit operator Rectangle(DxRectangle rectangle)
        {
            return new Rectangle((int) Math.Round(rectangle.X), (int) Math.Round(rectangle.Y),
                (int) Math.Round(rectangle.Width), (int) Math.Round(rectangle.Height));
        }

        public static implicit operator DxRectangle(Rectangle rectangle)
        {
            return new DxRectangle(rectangle);
        }

        public static DxRectangle FromRange(DxVector2 source, float radius)
        {
            return new DxRectangle(source.X - radius, source.Y - radius, radius * 2, radius * 2);
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
            return (lhs.X.FuzzyCompare(rhs.X, Tolerance) == 0) && (lhs.Y.FuzzyCompare(rhs.Y, Tolerance) == 0) &&
                   (lhs.Width.FuzzyCompare(rhs.Width, Tolerance) == 0) &&
                   (lhs.Height.FuzzyCompare(rhs.Height, Tolerance) == 0);
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

        public static DxRectangle operator +(DxRectangle lhs, DxVector2 translation)
        {
            lhs.X += translation.X;
            lhs.Y += translation.Y;
            return lhs;
        }

        public static DxRectangle operator -(DxRectangle lhs, DxVector2 translation)
        {
            lhs.X -= translation.X;
            lhs.Y -= translation.Y;
            return lhs;
        }

        public static DxRectangle operator +(DxRectangle lhs, DxRectangle rhs)
        {
            lhs.X += rhs.X;
            lhs.Y += rhs.Y;
            lhs.Width += rhs.Width;
            lhs.Height += rhs.Height;
            return lhs;
        }

        public bool Contains(float x, float y)
        {
            return (X <= x) && (x < X + Width) && (Y <= y) && (y < Y + Height);
        }

        public bool Contains(DxRectangle rectangle)
        {
            if((X <= rectangle.X) && (rectangle.X + rectangle.Width <= X + Width) && (Y <= rectangle.Y))
            {
                return rectangle.Y + rectangle.Height <= Y + Height;
            }
            return false;
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
            return (rectangle.Left < Right) && (Left < rectangle.Right) && (rectangle.Top < Bottom) &&
                   (Top < rectangle.Bottom);
        }

        public bool Intersects(DxRectangle rectangle)
        {
            return (rectangle.Left < Right) && (Left < rectangle.Right) && (rectangle.Top < Bottom) &&
                   (Top < rectangle.Bottom);
        }

        public List<DxRectangle> Divide(float maxWidth, float maxHeight)
        {
            var numXSections = Width / maxWidth;
            var numYSections = Height / maxHeight;
            List<DxRectangle> dividedRegions = new List<DxRectangle>((int) Math.Max(1, numXSections * numYSections));
            for(int i = 0; i < numXSections; ++i)
            {
                float startX = X + maxWidth * i;
                float endX = X + Math.Min(maxWidth * (i + 1), Width);
                for(int j = 0; j < numYSections; ++j)
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
            if(lhs.Intersects(rhs))
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
            if(lhs.Intersects(rhs))
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
            return rhs is DxRectangle && Equals((DxRectangle) rhs);
        }

        public override string ToString()
        {
            return $"{{ X:{X:N2} Y:{Y:N2} Width:{Width:N2} Height:{Height:N2} }}";
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(X, Y, Width, Height);
        }
    }
}