using System;

namespace DXGame.Core.Utils
{
    public struct DxRectangle : IEquatable<DxRectangle>
    {
        private static DxRectangle emptyRectangle = new DxRectangle();

        public float X;

        public float Y;

        public float Width;

        public float Height;

        public DxRectangle EmptyRectangle
        {
            get { return emptyRectangle; }
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

        static DxRectangle()
        {
        }

        public DxRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static bool operator ==(DxRectangle lhs, DxRectangle rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Width == rhs.Width &&
                   lhs.Height == rhs.Height;
        }

        public static bool operator !=(DxRectangle lhs, DxRectangle rhs)
        {
            return lhs.X != rhs.X || lhs.Y != rhs.Y || lhs.Width != rhs.Width ||
                   lhs.Height != rhs.Height;
        }

        public bool Contains(float x, float y)
        {
            if (X <= x && x < X + Width && Y <= y)
            {
                return y < Y + Height;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return false; // TODO
        }

        public override int GetHashCode()
        {
            return 0; // TODO
        }

        public override string ToString()
        {
            return ""; // TODO
        }

        public bool Equals(DxRectangle rhs)
        {
            return false; // TODO
        }
    }
}