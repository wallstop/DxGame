using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.Map
{
    /**
        <summary>
            Intended-to-be serializable description of a Map. 
            This should generally translate 1:1 with loaded Map objects. 
            Stored nice and json-ey.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class MapDescriptor : JsonPersistable<MapDescriptor>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public static string MapExtension => ".mdtr";

        [IgnoreDataMember]
        public override string Extension => MapExtension;

        [IgnoreDataMember]
        public override MapDescriptor Item => this;

        /* In indices */

        [DataMember]
        public int Width { get; }

        /* In indices */

        [DataMember]
        public int Height { get; }

        /* In units */

        [DataMember]
        public int TileWidth { get; }

        /* In units */

        [DataMember]
        public int TileHeight { get; }

        [IgnoreDataMember]
        public DxRectangle Bounds => new DxRectangle(0, 0, Width * TileWidth, Height * TileHeight);

        /* TODO: Make readonly? */

        [DataMember]
        public Dictionary<TilePosition, Tile> Tiles { get; }

        private MapDescriptor(Dictionary<TilePosition, Tile> tiles, int width, int height, int tileWidth, int tileHeight)
        {
            LOG.Info("Created a map that's {0} by {1} with {2} tiles of size ({3}, {4})", width, height, tiles.Count,
                tileWidth, tileHeight);
            Tiles = tiles;
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        public static MapDescriptorBuilder Builder()
        {
            return new MapDescriptorBuilder();
        }

        public class MapDescriptorBuilder : IBuilder<MapDescriptor>
        {
            private static readonly int INVALID_SIZE = -1;

            private int width_ = INVALID_SIZE;
            private int height_ = INVALID_SIZE;
            private int tileWidth_ = INVALID_SIZE;
            private int tileHeight_ = INVALID_SIZE;
            private readonly Dictionary<TilePosition, Tile> tiles_ = new Dictionary<TilePosition, Tile>();

            public MapDescriptorBuilder WithTile(TilePosition tilePosition, Tile tile)
            {
                Validate.Hard.IsTrue(0 <= tilePosition.X);
                Validate.Hard.IsTrue(0 <= tilePosition.Y);
                Validate.Hard.IsNotNull(tile);
                if(tiles_.ContainsKey(tilePosition))
                {
                    LOG.Info("Overwriting tile at {0}", tilePosition);
                }

                tiles_[tilePosition] = tile;
                return this;
            }

            public MapDescriptorBuilder WithTile(int x, int y, Tile tile)
            {
                return WithTile(new TilePosition(x, y), tile);
            }

            public MapDescriptorBuilder WithWidth(int width)
            {
                width_ = width;
                return this;
            }

            public MapDescriptorBuilder WithHeight(int height)
            {
                height_ = height;
                return this;
            }

            public MapDescriptorBuilder WithTileWidth(int tileWidth)
            {
                tileWidth_ = tileWidth;
                return this;
            }

            public MapDescriptorBuilder WithTileHeight(int tileHeight)
            {
                tileHeight_ = tileHeight;
                return this;
            }

            public MapDescriptor Build()
            {
                Validate.Hard.IsTrue(0 < width_, () => $"Cannot create a {typeof(MapDescriptor)} with a width of {width_}");
                Validate.Hard.IsTrue(0 < height_, () => $"Cannot create a {typeof(MapDescriptor)} with a height of {height_}");
                Validate.Hard.IsTrue(0 < tileWidth_,
                    () => $"Cannot create a {typeof(MapDescriptor)} with a tile width of {tileWidth_}");
                Validate.Hard.IsTrue(0 < tileHeight_,
                    () => $"Cannot create a {typeof(MapDescriptor)} with a tile  height of {tileHeight_}");
                Validate.Hard.IsNotEmpty(tiles_, () => $"Cannot create a {typeof(MapDescriptor)} without any tiles");
                return new MapDescriptor(tiles_.ToDictionary(), width_, height_, tileWidth_, tileHeight_);
            }
        }
    }
}