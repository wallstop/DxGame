using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Utils
{
    public static class RectangleUtils
    {
        public static Rectangle Add(this Rectangle rectangle, Point point)
        {
            return new Rectangle(point, rectangle.Size);
        }
    }
}
