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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string MapExtension => ".mdtr";

        [IgnoreDataMember]
        public override string Extension => MapExtension;

        [IgnoreDataMember]
        public override MapDescriptor Item => this;

        [DataMember]
        public MapLayout MapLayout { get; }

        /* In indices */

        [IgnoreDataMember]
        public int Width => MapLayout.Width;

        /* In indices */

        [IgnoreDataMember]
        public int Height => MapLayout.Height;

        /* In units */

        [IgnoreDataMember]
        public int TileWidth => MapLayout.TileWidth;

        /* In units */

        [IgnoreDataMember]
        public int TileHeight => MapLayout.TileHeight;

        [IgnoreDataMember]
        public DxRectangle Bounds => MapLayout.Bounds;

        /* TODO: Make readonly? */

        [DataMember]
        public Dictionary<TilePosition, Tile> Tiles { get; }

        private MapDescriptor(Dictionary<TilePosition, Tile> tiles, MapLayout mapLayout)
        {
            Logger.Info("Created a map that's {0} by {1} with {2} tiles of size ({3}, {4})", mapLayout.Width,
                mapLayout.Height, tiles.Count, mapLayout.TileWidth, mapLayout.TileHeight);
            Tiles = tiles;
            MapLayout = mapLayout;
        }

        public static MapDescriptorBuilder Builder()
        {
            return new MapDescriptorBuilder();
        }

        public class MapDescriptorBuilder : IBuilder<MapDescriptor>
        {
            private readonly MapLayout.MapLayoutBuilder mapLayoutBuilder_ = new MapLayout.MapLayoutBuilder();

            private readonly Dictionary<TilePosition, Tile> tiles_ = new Dictionary<TilePosition, Tile>();

            public MapDescriptorBuilder WithTile(TilePosition tilePosition, Tile tile)
            {
                Validate.Hard.IsTrue(0 <= tilePosition.X);
                Validate.Hard.IsTrue(0 <= tilePosition.Y);
                Validate.Hard.IsNotNull(tile);
                if(tiles_.ContainsKey(tilePosition))
                {
                    Logger.Info("Overwriting tile at {0}", tilePosition);
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
                mapLayoutBuilder_.WithWidth(width);
                return this;
            }

            public MapDescriptorBuilder WithHeight(int height)
            {
                mapLayoutBuilder_.WithHeight(height);
                return this;
            }

            public MapDescriptorBuilder WithTileWidth(int tileWidth)
            {
                mapLayoutBuilder_.WithTileWidth(tileWidth);
                return this;
            }

            public MapDescriptorBuilder WithTileHeight(int tileHeight)
            {
                mapLayoutBuilder_.WithTileHeight(tileHeight);
                return this;
            }

            public MapDescriptorBuilder WithMapLayout(MapLayout mapLayout)
            {
                mapLayoutBuilder_.WithMapLayout(mapLayout);
                return this;
            }

            public MapDescriptor Build()
            {
                MapLayout mapLayout = mapLayoutBuilder_.Build();
                Validate.Log.IsNotEmpty(tiles_, () => $"Creating {typeof(MapDescriptor)} without any tiles");
                return new MapDescriptor(tiles_.ToDictionary(), mapLayout);
            }
        }
    }
}