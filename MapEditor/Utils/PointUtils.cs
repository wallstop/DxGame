using System.Drawing;

namespace MapEditor.Utils
{
    public static class PointUtils
    {
        public static Point Add(this Point source, Point offset)
        {
            return new Point(source.X + offset.X, source.Y + offset.Y);
        }

        public static Point Subtract(this Point source, Point offset)
        {
            return new Point(offset.X - source.X, offset.Y - source.Y);
        }
    }
}