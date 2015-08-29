using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NetTopologySuite.IO;

namespace DxGameUtils.Core
{
    public class QoraAnimationMaker
    {
        public static void Convert(string path)
        {
            var filesInDirectory = Directory.GetDirectories(path);


            foreach (var directory in filesInDirectory)
            {
                Console.WriteLine($"Discovered {directory} in {path}");
                if (Directory.Exists(directory))
                {
                    ProcessAnimation(directory);
                }
                else
                {
                    Console.WriteLine($"Could not resolve {directory} to be a valid directory");
                }
            }
        }

        private static void ProcessAnimation(string folder)
        {
            var filesInDirectory = Directory.GetFiles(folder);
            var left =
                filesInDirectory.Where(
                    file => Path.GetExtension(file) == ".png" && (Path.GetFileNameWithoutExtension(file)?.StartsWith("L") ?? false))
                    .ToList();
            var right = filesInDirectory.Where(
                    file => Path.GetExtension(file) == ".png" && (Path.GetFileNameWithoutExtension(file)?.StartsWith("R") ?? false))
                    .ToList();

            var others =
                filesInDirectory.Where(
                    file => Path.GetExtension(file) == ".png" && !left.Contains(file) && !right.Contains(file)).ToList();

            left.Sort();
            var name = Path.GetFileNameWithoutExtension(folder);
            if (left.Any())
            {
                CreateAnimation(left, name + "_Left");
            }
            right.Sort();
            if (right.Any())
            {
                CreateAnimation(right, name + "_Right");
            }

            others.Sort();
            if (others.Any())
            {
                CreateAnimation(others, name);
            }
        }

        private static void CreateAnimation(List<string> animationFiles, string animationName)
        {
            var output = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Output" +
                         Path.DirectorySeparatorChar;

            /* Bitmaps have a weird issue with transparency, so just grab the "pixel format" from the first loaded qora image and use that throughout */
            using (var firstFrame = Image.FromFile(animationFiles.First()))
            {
                var targetWidth = firstFrame.Width * animationFiles.Count;
                var targetHeight = firstFrame.Height;
                using (
                    Image bitmap = new Bitmap(targetWidth, targetHeight,
                        firstFrame.PixelFormat))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImage(firstFrame, new Point(0, 0));
                        for (int i = 1; i < animationFiles.Count; ++i)
                        {
                            using (var animationFrame = Image.FromFile(animationFiles[i]))
                            {
                                graphics.DrawImage(animationFrame, new Point(animationFrame.Width * i, 0));
                            }

                        }
                        graphics.Flush(FlushIntention.Sync);
                        graphics.Save();
                    }

                    if (!Directory.Exists(output))
                    {
                        Directory.CreateDirectory(output);
                    }
                    bitmap.Save(output + animationName + ".png", ImageFormat.Png);
                    Console.WriteLine($"Created {animationName} for {animationFiles.Count} animationFrames");
                }
            }
        }

    }
}
