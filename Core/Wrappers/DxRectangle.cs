using System;
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

        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }

        [DataMember]
        public float Width { get; set; }

        [DataMember]
        public float Height { get; set; }

        public float Area
        {
            get { return Width * Height; }
        }

        public DxRectangle EmptyRectangle
        {
            get { return new DxRectangle(); }
        }

        public float Left
        {
            get { return X; }
        }

        public float Right
        {
            get { return X + Width; }
        }

        public float Top
        {
            get { return Y; }
        }

        public float Bottom
        {
            get { return Y + Height; }
        }

        public Point Center
        {
            get { return new Point((int) ((X + Width) / 2), (int) ((Y + Height) / 2)); }
        }

        public DxRectangle(Vector2 x, Vector2 y) : this()
        {
            X = x.X;
            Width = x.Y;
            Y = y.X;
            Height = y.Y;
        }

        public DxRectangle(DxVector2 x, DxVector2 y) : this()
        {
            X = x.X;
            Width = x.Y;
            Y = y.X;
            Height = y.Y;
        }

        public DxRectangle(Rectangle rectangle) : this()
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public DxRectangle(float x, float y, float width, float height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Equals(DxRectangle rhs) { return this == rhs; }
        public bool Equals(Rectangle other) { return X.Equals(other.X) && Y.Equals(other.Y); }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int) X, (int) Y, (int) Width, (int) Height);
        }

        public static DxRectangle FromRectangle(Rectangle rectangle)
        {
            return new DxRectangle(rectangle);
        }

        public Vector2 XY() { return new Vector2(X, Y); }

        public static bool operator ==(DxRectangle lhs, DxRectangle rhs)
        {
            return MathUtils.FuzzyCompare(lhs.X, rhs.X, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Y, rhs.Y, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Width, rhs.Width, TOLERANCE) == 0
                   && MathUtils.FuzzyCompare(lhs.Height, rhs.Height, TOLERANCE) == 0;
        }

        public static bool operator !=(DxRectangle lhs, DxRectangle rhs)
        {
            return MathUtils.FuzzyCompare(lhs.X, rhs.X, TOLERANCE) != 0
                   && MathUtils.FuzzyCompare(lhs.Y, rhs.Y, TOLERANCE) != 0
                   && MathUtils.FuzzyCompare(lhs.Width, rhs.Width, TOLERANCE) != 0
                   && MathUtils.FuzzyCompare(lhs.Height, rhs.Height, TOLERANCE) != 0;
        }

        public bool Contains(float x, float y)
        {
            if (X <= x && x < (X + Width) && Y <= y)
            {
                return y < (Y + Height);
            }
            return false;
        }

        public bool Contains(Vector2 point) { return Contains(point.X, point.Y); }
        public bool Contains(Point point) { return Contains(point.X, point.Y); }

        public bool Intersects(Rectangle rectangle)
        {
            if (MathUtils.FuzzyCompare(rectangle.Left, Right, TOLERANCE) < 0
                && MathUtils.FuzzyCompare(Left, rectangle.Right, TOLERANCE) < 0
                && MathUtils.FuzzyCompare(rectangle.Top, Bottom) < 0)
            {
                return MathUtils.FuzzyCompare(Top, rectangle.Bottom, TOLERANCE) < 0;
            }

            return false;
        }

        public bool Intersects(DxRectangle rectangle)
        {
            if (MathUtils.FuzzyCompare(rectangle.Left, Right, TOLERANCE) < 0
                && MathUtils.FuzzyCompare(Left, rectangle.Right, TOLERANCE) < 0
                && MathUtils.FuzzyCompare(rectangle.Top, Bottom) < 0)
            {
                return MathUtils.FuzzyCompare(Top, rectangle.Bottom, TOLERANCE) < 0;
            }

            return false;
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
                // Suprres any class cast exceptions
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("{{X:{0:N2} Y:{1:N2} Width:{2:N2} Height:{3:N2}", X, Y, Width,
                Height);
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