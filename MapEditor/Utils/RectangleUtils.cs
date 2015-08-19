using System.Drawing;

namespace MapEditor.Utils
{
    public static class RectangleUtils
    {
        public static Rectangle Add(this Rectangle rectangle, Point point)
        {
            return new Rectangle(point, rectangle.Size);
        }

        public static Rectangle Multiply(this Rectangle rectangle, double scale)
        {
            return new Rectangle(rectangle.X, rectangle.Y, (int) (rectangle.Width * scale),
                (int) (rectangle.Height * scale));
        }
    }
}