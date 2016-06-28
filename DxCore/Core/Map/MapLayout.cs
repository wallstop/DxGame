using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapLayout
    {
        /* In Tiles */

        [DataMember]
        public int Width { get; private set; }

        /* In Tiles */

        [DataMember]
        public int Height { get; private set; }

        /* In Units */

        [DataMember]
        public int TileWidth { get; private set; }

        /* In Units */

        [DataMember]
        public int TileHeight { get; private set; }

        [IgnoreDataMember]
        public DxRectangle Bounds => new DxRectangle(0, 0, Width * TileWidth, Height * TileHeight);

        private MapLayout(int width, int height, int tileWidth, int tileHeight)
        {
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        public static MapLayoutBuilder Builder()
        {
            return new MapLayoutBuilder();
        }

        public class MapLayoutBuilder : IBuilder<MapLayout>
        {
            private const int Invalid = -1;

            private int width_ = Invalid;
            private int height_ = Invalid;
            private int tileWidth_ = Invalid;
            private int tileHeight_ = Invalid;

            public MapLayoutBuilder WithWidth(int width)
            {
                width_ = width;
                return this;
            }

            public MapLayoutBuilder WithHeight(int height)
            {
                height_ = height;
                return this;
            }

            public MapLayoutBuilder WithTileWidth(int tileWidth)
            {
                tileWidth_ = tileWidth;
                return this;
            }

            public MapLayoutBuilder WithTileSize(int tileSize)
            {
                WithTileWidth(tileSize);
                WithTileHeight(tileSize);
                return this;
            }

            public MapLayoutBuilder WithTileHeight(int tileHeight)
            {
                tileHeight_ = tileHeight;
                return this;
            }

            public MapLayoutBuilder WithMapLayout(MapLayout mapLayout)
            {
                Validate.Hard.IsNotNull(mapLayout);
                width_ = mapLayout.Width;
                height_ = mapLayout.Height;
                tileWidth_ = mapLayout.TileWidth;
                tileHeight_ = mapLayout.TileHeight;
                return this;
            }

            public MapLayout Build()
            {
                Validate.Hard.IsPositive(width_);
                Validate.Hard.IsPositive(height_);
                Validate.Hard.IsPositive(tileWidth_);
                Validate.Hard.IsPositive(tileHeight_);
                return new MapLayout(width_, height_, tileWidth_, tileHeight_);
            }
        }
    }
}
