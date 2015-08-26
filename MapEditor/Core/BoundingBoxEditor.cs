using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;

namespace MapEditor.Core
{
    public class BoundingBoxEditor
    {
        private static readonly float MAX_COLLIDABLE_WIDTH = 50;
        private static readonly float MAX_COLLIDABLE_HEIGHT = 50;
        private readonly MapDescriptor descriptor_;
        private readonly List<DxRectangle> originalBoundaries_ = new List<DxRectangle>();
        private RTree<Platform> tree_;
        private double zoomFactor_ = 1.0;

        private BoundingBox<Platform> PlatformBounds
        {
            get { return platform => platform.BoundingBox; }
        }

        public IEnumerable<DxRectangle> Collidables => tree_.Rectangles;
        public IEnumerable<DxRectangle> OriginalScaleBoundaries => originalBoundaries_;

        public BoundingBoxEditor()
        {
            tree_ = new RTree<Platform>(PlatformBounds, new List<Platform>());
            descriptor_ = new MapDescriptor();
        }

        private double Zoom()
        {
            return zoomFactor_;
        }

        public void Add(DxRectangle area)
        {
            originalBoundaries_.Add(area);
            ResetTree();
        }

        public void RemoveInRange(DxRectangle range)
        {
            var platformsInRange = tree_.InRange(range);
            originalBoundaries_.RemoveAll(
                boundary =>
                    platformsInRange.Select(platform => platform.BoundingBox)
                        .Any(existing => existing.Intersects(boundary)));
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

            var outImage = new Bitmap(map, (int) (map.Width * zoomFactor_), (int) (map.Height * zoomFactor_));
            outImage.Save(baseName + mapImageFileEnding, ImageFormat.Png);
            var descriptor = new MapDescriptor
            {
                Asset = simpleName + mapImageFileEnding,
                Platforms = descriptor_.Platforms,
                Size = new DxRectangle(0, 0, outImage.Width, outImage.Height)
            };

            descriptor.Save(fileName);
        }

        private void ResetTree()
        {
            var transformedBoundaries = OriginalScaleBoundaries.SelectMany(boundary => (boundary * Zoom())
                .Divide(MAX_COLLIDABLE_WIDTH, MAX_COLLIDABLE_HEIGHT)).Select(boundary => boundary / Zoom()).ToList();
            originalBoundaries_.Clear();
            originalBoundaries_.AddRange(transformedBoundaries);

            var platforms = OriginalScaleBoundaries.Select(boundary => new Platform(boundary)
            {
                CollidableDirections =
                    new List<CollidableDirection>(
                        Enum.GetValues(typeof (CollidableDirection)).ToEnumerable<CollidableDirection>())
            }).ToList();
            descriptor_.Platforms = platforms.Select(platform => new
                Platform(platform.BoundingBox * Zoom())
            {
                CollidableDirections = platform.CollidableDirections,
                Type = platform.Type
            }).ToList();
            tree_ = new RTree<Platform>(PlatformBounds, platforms);
        }
    }
}