using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DxCore.Core;
using DxCore.Core.Map;
using DxCore.Core.Primitives;

namespace DxGameUtils.Core
{
    /*
        Converts old style text-layout to new style full-map images
    */

    public class TextFileToMapImageConverter
    {
        private static readonly int BLOCK_SIZE = 50;

        private static Dictionary<char, Bitmap> BlockLookup => new Dictionary<char, Bitmap>
        {
            {'R', new Bitmap("Content/Map/Blocks/RedBlock.png")},
            {'G', new Bitmap("Content/Map/Blocks/GreenBlock.png")},
            {'B', new Bitmap("Content/Map/Blocks/BlueBlock.png")},
            {'P', new Bitmap("Content/Map/Blocks/PurpleBlock.png")},
            {'Y', new Bitmap("Content/Map/Blocks/YellowBlock.png")},
            {'O', new Bitmap("Content/Map/Blocks/OrangeBlock.png")}
        };

        public static void Convert(string pathToTextFile, string outputDirectory, string mapName = "Generated.png",
            string descriptorName = "GeneratedMapDescriptor.mdtr")
        {
            List<string> fileContents = new List<string>();

            using (StreamReader fileReader = new StreamReader(pathToTextFile))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    fileContents.Add(line);
                }
            }

            int width = fileContents.Max(line => line.Length) * BLOCK_SIZE;
            int height = fileContents.Count * BLOCK_SIZE;

            MapDescriptor descriptor = new MapDescriptor {Size = new DxRectangle(0, 0, width, height), Asset = mapName};

            var blockLookup = BlockLookup;

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.WhiteSmoke);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    for (int y = 0; y < fileContents.Count; ++y)
                    {
                        for (int x = 0; x < fileContents[y].Length; ++x)
                        {
                            char currentCharacter = fileContents[y][x];
                            if (!blockLookup.ContainsKey(currentCharacter))
                            {
                                continue;
                            }
                            Bitmap mapBlock = blockLookup[currentCharacter];
                            var mapX = x * BLOCK_SIZE;
                            var mapY = y * BLOCK_SIZE;
                            graphics.DrawImage(mapBlock, new Point(mapX, mapY));

                            Platform block = new Platform(new DxRectangle(mapX, mapY, BLOCK_SIZE, BLOCK_SIZE));

                            descriptor.Platforms.Add(block);
                        }
                    }
                    graphics.Flush(FlushIntention.Sync);
                    graphics.Save();
                }

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                bitmap.Save(outputDirectory + mapName, ImageFormat.Png);
                Serializer<MapDescriptor>.WriteToJsonFile(descriptor, outputDirectory + descriptorName);
            }
        }
    }
}