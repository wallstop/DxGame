using System.Drawing;

namespace MapEditor.Utils
{
    public static class RectangleUtils
    {
        public static Rectangle Add(this Rectangle rectangle, Point point)
        {
            return new Rectangle(new Point(point.X + rectangle.X, point.Y + rectangle.Y), rectangle.Size);
        }

        public static Rectangle Subtract(this Rectangle rectangle, Point point)
        {
            return new Rectangle(new Point(rectangle.X - point.X, rectangle.Y - point.Y), rectangle.Size);
        }

        public static Rectangle Multiply(this Rectangle rectangle, double scale)
        {
            return new Rectangle(rectangle.X, rectangle.Y, (int) (rectangle.Width * scale),
                (int) (rectangle.Height * scale));
        }
    }
}