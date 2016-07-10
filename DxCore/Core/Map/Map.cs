using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Map;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Cache.Simple;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class Map : DrawableComponent
    {
        [DataMember] private MapDescriptor mapDescriptor_;

        [IgnoreDataMember]
        public MapDescriptor MapDescriptor
        {
            get { return mapDescriptor_; }
            set
            {
                mapDescriptor_ = value;
                new UpdateCameraBounds(mapDescriptor_.Bounds).Emit();
            }
        }

        [DataMember]
        private Dictionary<TilePosition, MapTile> MapTiles { get; set; }

        [DataMember]
        public ISpatialTree<MapTile> TileSpatialTree { get; private set; }

        [DataMember]
        public DxVector2 PlayerSpawn { get; private set; }

        [IgnoreDataMember] [NonSerialized] private readonly ISimpleCache<Tile, Texture2D> tileTextureCache_ =
            new UnboundedLoadingSimpleCache<Tile, Texture2D>(tile => DxGame.Instance.Content.Load<Texture2D>(tile.Asset));

        // TODO: Have spawn locations be a part of the Map Descriptor
        public DxRectangle RandomSpawnLocation
        {
            get
            {
                DxRectangle boundary = MapDescriptor.Bounds;
                DxRectangle spawnLocation;
                do
                {
                    /* Pick a random place that has at least a 500x500 area of "no map collidables" */
                    /* TODO: Have this be some kind of chooseable algorithm or something (bundled in map descriptor? ) */
                    spawnLocation =
                        new DxRectangle(
                            ThreadLocalRandom.Current.NextFloat(boundary.X, boundary.X + boundary.Width - 250),
                            ThreadLocalRandom.Current.NextFloat(boundary.Y, boundary.Y + boundary.Height - 250), 250,
                            250);
                } while(CollidesWithMap(spawnLocation));
                return spawnLocation;
            }
        }

        public static Dictionary<TilePosition, MapTile> DetermineMapTiles(MapDescriptor mapDescriptor)
        {
            Validate.Hard.IsNotNullOrDefault(mapDescriptor);
            Dictionary<TilePosition, MapTile> mapTiles = new Dictionary<TilePosition, MapTile>(mapDescriptor.Tiles.Count);

            foreach(KeyValuePair<TilePosition, Tile> tilePair in mapDescriptor.Tiles)
            {
                TilePosition position = tilePair.Key;
                Tile tile = tilePair.Value;
                mapTiles[position] = new MapTile(tile,
                    new DxRectangle(position.X * mapDescriptor.TileWidth, position.Y * mapDescriptor.TileHeight,
                        mapDescriptor.TileWidth, mapDescriptor.TileHeight));
            }
            return mapTiles;
        }

        public Map(MapDescriptor descriptor)
        {
            Validate.Hard.IsNotNullOrDefault(descriptor, this.GetFormattedNullOrDefaultMessage(nameof(descriptor)));
            MapDescriptor = descriptor;

            MapTiles = DetermineMapTiles(descriptor);
            TileSpatialTree = new RTree<MapTile>(mapTile => mapTile.Space, MapTiles.Values.ToList());
            DrawPriority = DrawPriority.Map;

            PlayerSpawn = RandomSpawnLocation.Position;
        }

        public override void LoadContent()
        {
            foreach(MapTile mapTile in MapTiles.Values)
            {
                /* Loading cache - textures should be loaded in automagically */
                tileTextureCache_.Get(mapTile.Tile);
            }

            base.LoadContent();
        }

        public override void Initialize()
        {
            MapGeometryComponent mapGeometry =
                new MapGeometryComponent(MapTiles.Values.Select(tile => tile.Space).ToList());
            mapGeometry.Create();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            DxRectangle range = DxGame.Instance.ScreenRegion;
            range.X *= -1;
            range.Y *= -1;

            List<MapTile> mapTilesOnScreen = TileSpatialTree.InRange(range);
            foreach(MapTile mapTile in mapTilesOnScreen)
            {
                spriteBatch.Draw(tileTextureCache_.Get(mapTile.Tile), null, mapTile.Space);
            }
        }

        private bool CollidesWithMap(DxRectangle region)
        {
            List<MapTile> collisions = TileSpatialTree.InRange(region);
            return collisions.Any(collidable => collidable.Space.Intersects(region));
        }
    }
}