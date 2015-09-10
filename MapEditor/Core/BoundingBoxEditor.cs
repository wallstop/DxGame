using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;

namespace MapEditor.Core
{
    public class BoundingBoxEditor
    {
        private readonly MapDescriptor descriptor_;

        private readonly List<Tuple<DxRectangle, PlatformType>> originalBoundaries_ =
            new List<Tuple<DxRectangle, PlatformType>>();
        private RTree<Platform> tree_;
        private double zoomFactor_ = 1.0;

        private BoundingBox<Platform> PlatformBounds
        {
            get { return platform => platform.BoundingBox; }
        }

        public IEnumerable<DxRectangle> Collidables => tree_.Divisions;
        public IEnumerable<Tuple<DxRectangle, PlatformType>> OriginalScaleBoundaries => originalBoundaries_;

        public BoundingBoxEditor()
        {
            tree_ = new RTree<Platform>(PlatformBounds, new List<Platform>());
            descriptor_ = new MapDescriptor();
        }

        private double Zoom()
        {
            return zoomFactor_;
        }

        public void Add(DxRectangle area, PlatformType type)
        {
            originalBoundaries_.Add(Tuple.Create(area, type));
            ResetTree();
        }

        public void RemoveInRange(DxRectangle range)
        {
            var platformsInRange = tree_.InRange(range);
            originalBoundaries_.RemoveAll(
                boundary =>
                    platformsInRange.Select(platform => platform.BoundingBox)
                        .Any(existing => existing.Intersects(boundary.Item1)));
            ResetTree();
        }

        public void Clear()
        {
            zoomFactor_ = 1.0;
            originalBoundaries_.Clear();
            ResetTree();
        }

        public void Resize(double zoomFactor)
        {
            zoomFactor_ = zoomFactor;
            ResetTree();
        }

        public void Save(string fileName, Image map)
        {
            /* TODO: Take into account collidable directions */
            var simpleName = Path.GetFileNameWithoutExtension(fileName);
            var directory = Path.GetDirectoryName(fileName);
            var baseName = directory + Path.DirectorySeparatorChar + simpleName;
            var mapImageFileEnding = ".png";

            var outImage = new Bitmap(map);
            outImage.Save(baseName + mapImageFileEnding, ImageFormat.Png);
            var descriptor = new MapDescriptor
            {
                Asset = simpleName + mapImageFileEnding,
                /* There's a subtle bug elsewhere that is causing 0-size rectangles to be created, so simply ignore them for now (bug hunt later) */
                Platforms = descriptor_.Platforms.Where(platform => platform.BoundingBox.Width > 0 && platform.BoundingBox.Height > 0).ToList(),
                Size = new DxRectangle(0, 0, outImage.Width, outImage.Height),
                Scale = (float)Zoom()
            };

            descriptor.Save(fileName);
        }

        private void ResetTree()
        {
            var platforms = OriginalScaleBoundaries.Select(boundary => new Platform(boundary.Item1, boundary.Item2)).ToList();
            descriptor_.Platforms = platforms.Select(platform =>
            {
                var copy = new Platform(platform);
                copy.BoundingBox = copy.BoundingBox * Zoom();
                return copy;
            }).ToList();
            tree_ = new RTree<Platform>(PlatformBounds, platforms);
        }
    }
}