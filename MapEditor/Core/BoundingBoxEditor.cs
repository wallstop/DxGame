using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;

namespace MapEditor.Core
{
    public class BoundingBoxEditor
    {
        private readonly MapDescriptor descriptor_;
        private RTree<Platform> tree_;
        public IEnumerable<DxRectangle> Collidables => descriptor_.Platforms.Select(platform => platform.BoundingBox);

        public BoundingBoxEditor()
        {
            tree_ = new RTree<Platform>(platform => platform.BoundingBox, new List<Platform>());
            descriptor_ = new MapDescriptor();
        }

        public void Add(IEnumerable<DxRectangle> regions)
        {
            descriptor_.Platforms.AddRange(regions.Select(region => new Platform(region)));
            ResetTree();
        }

        public void RemoveInRange(DxRectangle range)
        {
            var platformsInRange = tree_.InRange(range);
            descriptor_.Platforms.RemoveAll(platform => platformsInRange.Contains(platform));
            ResetTree();
        }

        public void Clear()
        {
            ResetTree();
            descriptor_.Platforms.Clear();
        }

        public void Save(string fileName, Image map, float scalar)
        {
            /* TODO: Take into account collidable directions */
            var collidables =
                descriptor_.Platforms.Select(platform => new Platform(platform.BoundingBox * scalar)).ToList();
            var simpleName = Path.GetFileNameWithoutExtension(fileName);
            var directory = Path.GetDirectoryName(fileName);
            var baseName = directory + Path.DirectorySeparatorChar + simpleName;
            var mapImageFileEnding = ".png";

            var outImage = new Bitmap(map, (int) (map.Width * scalar), (int) (map.Height * scalar));
            outImage.Save(baseName + mapImageFileEnding, ImageFormat.Png);
            var descriptor = new MapDescriptor
            {
                Asset = simpleName + mapImageFileEnding,
                Platforms = collidables,
                Size = new DxRectangle(0, 0, outImage.Width, outImage.Height)
            };

            descriptor.Save(fileName);
        }

        private void ResetTree()
        {
            tree_ = new RTree<Platform>(platform => platform.BoundingBox, descriptor_.Platforms);
        }
    }
}