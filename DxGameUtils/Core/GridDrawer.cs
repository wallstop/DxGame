using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxGameUtils.Core
{
    public class GridDrawer
    {
        private const int GRID_WIDTH = 25; 

        public static void DrawGrid(string imageFile, int gridWidth = GRID_WIDTH)
        {
            using (Bitmap image = new Bitmap(imageFile))
            {
                var width = image.Width;
                var height = image.Height;
                Pen redPen = new Pen(Color.DarkRed, 0.5f);

                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.Bicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    var font = new Font("Courier", 12);

                    for (int i = 0; i < width; i += gridWidth)
                    {
                        var fontRectangle = new RectangleF(i, 0, Math.Min(i + 15, width), 15);
                        graphics.DrawLine(redPen, i, 5, i, height - 5);
                        graphics.DrawString(i.ToString(), font, Brushes.Black, fontRectangle);
                    }
                    for (int j = 0; j < height; j += gridWidth)
                    {
                        var fontRectangle = new RectangleF(0, j, 100, 15);
                        graphics.DrawLine(redPen, 5, j, width - 5, j);
                        graphics.DrawString(j.ToString(), font, Brushes.Black, fontRectangle);
                    }
                    graphics.Flush();
                }
                var newImageFile = Path.GetFileNameWithoutExtension(imageFile) + "1";
                image.Save(newImageFile + ".png", ImageFormat.Png);
            }
        }
    }
}
