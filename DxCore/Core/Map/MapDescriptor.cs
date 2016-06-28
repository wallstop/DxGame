using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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
        public static string MapExtension => ".mdtr";

        [IgnoreDataMember]
        public override string Extension => MapExtension;

        [IgnoreDataMember]
        public override MapDescriptor Item => this;

        [DataMember]
        public MapLayout MapLayout { get; private set; }

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
        // TODO: Figure out units, pls. This is pretty urgent.
        public DxRectangle Bounds => MapLayout.Bounds;

        [DataMember]
        public Dictionary<TilePosition, Tile> Tiles { get; private set; }

        private MapDescriptor(Dictionary<TilePosition, Tile> tiles, MapLayout mapLayout)
        {
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

            public Dictionary<TilePosition, Tile> Tiles => tiles_.ToDictionary();

            public MapDescriptorBuilder WithoutTiles()
            {
                tiles_.Clear();
                return this;
            }

            public MapDescriptorBuilder WithoutTile(TilePosition tilePosition)
            {
                Validate.Hard.IsNotNull(tilePosition);
                tiles_.Remove(tilePosition);
                return this;
            }

            public MapDescriptorBuilder WithTile(TilePosition tilePosition, Tile tile)
            {
                Validate.Hard.IsTrue(0 <= tilePosition.X);
                Validate.Hard.IsTrue(0 <= tilePosition.Y);
                Validate.Hard.IsNotNull(tile);
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

            public MapDescriptorBuilder WithMapDescriptor(MapDescriptor existingMap)
            {
                Validate.Hard.IsNotNull(existingMap);
                tiles_.Clear();
                foreach(KeyValuePair<TilePosition, Tile> tileAndPosition in existingMap.Tiles)
                {
                    tiles_.Add(tileAndPosition.Key, tileAndPosition.Value);
                }
                mapLayoutBuilder_.WithMapLayout(existingMap.MapLayout);
                return this;
            }

            public MapDescriptor Build()
            {
                MapLayout mapLayout = mapLayoutBuilder_.Build();
                return new MapDescriptor(tiles_.ToDictionary(), mapLayout);
            }
        }
    }
}