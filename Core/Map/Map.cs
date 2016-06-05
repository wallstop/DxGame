using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Cache.Simple;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class Map : DrawableComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public MapDescriptor MapDescriptor { get; set; }

        [DataMember]
        private Dictionary<TilePosition, MapCollidable> MapCollidables { get; }

        [DataMember]
        public ISpatialTree<MapCollidable> Collidables { get; }

        [DataMember]
        public DxVector2 PlayerSpawn { get; }

        [IgnoreDataMember]
        [NonSerialized]
        private readonly ISimpleCache<Tile, Texture2D> tileTextureCache_ = new UnboundedLoadingSimpleCache<Tile, Texture2D>(tile => DxGame.Instance.Content.Load<Texture2D>(tile.Asset));

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

        public static Dictionary<TilePosition, MapCollidable> DetermineCollidables(MapDescriptor mapDescriptor)
        {
            Validate.IsNotNullOrDefault(mapDescriptor);
            Dictionary<TilePosition, MapCollidable> mapCollidables = new Dictionary<TilePosition, MapCollidable>(mapDescriptor.Tiles.Count);

            foreach(KeyValuePair<TilePosition, Tile> tilePair in mapDescriptor.Tiles)
            {
                TilePosition position = tilePair.Key;
                Tile tile = tilePair.Value;
                mapCollidables[position] = new MapCollidable(tile,
                    new DxRectangle(position.X * mapDescriptor.TileWidth, position.Y * mapDescriptor.TileHeight,
                        mapDescriptor.TileWidth, mapDescriptor.TileHeight));
            }
            return mapCollidables;
        }

        public Map(MapDescriptor descriptor)
        {
            Validate.IsNotNullOrDefault(descriptor, this.GetFormattedNullOrDefaultMessage(nameof(descriptor)));
            MapDescriptor = descriptor;

            MapCollidables = DetermineCollidables(descriptor);
            Collidables = new RTree<MapCollidable>(mapCollidable => mapCollidable.Space, MapCollidables.Values.ToList());
            DrawPriority = DrawPriority.MAP;

            PlayerSpawn = RandomSpawnLocation.Position;
        }

        public override void LoadContent()
        {
            foreach(MapCollidable mapCollidable in MapCollidables.Values)
            {
                /* Loading cache - textures should be loaded in automagically */
                tileTextureCache_.Get(mapCollidable.Tile);
            }

            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            DxRectangle range = DxGame.Instance.ScreenRegion;

            List<MapCollidable> mapCollidablesOnScreen = Collidables.InRange(range);
            foreach(MapCollidable mapCollidable in mapCollidablesOnScreen)
            {
                spriteBatch.Draw(tileTextureCache_.Get(mapCollidable.Tile), null, mapCollidable.Space.ToRectangle());
            }

            ///* 
            //    Map is pretty special: We want actually want to draw whatever chunk of the map the screen is currently seeing. 
            //    However, we've already set up a matrix translation on our camera for every object. So, we need to undo the translation (just flip x & y)
            //*/
            //DxRectangle target = range;
            //target.X = -target.X;
            //target.Y = -target.Y;


            //foreach(var mapLayerEntry in MapTexturesAndLayers)
            //{
            //    Texture2D mapImage = mapLayerEntry.Key;
            //    int layer = mapLayerEntry.Value;
            //    spriteBatch.Draw(mapImage, target.ToRectangle(), (target / MapDescriptor.Scale).ToRectangle(),
            //        Color.White, 0, new Vector2(), SpriteEffects.None, layer);
            //}
        }

        private bool CollidesWithMap(DxRectangle region)
        {
            List<MapCollidable> collisions = Collidables.InRange(region);
            return collisions.Any(collidable => collidable.Space.Intersects(region));
        }
    }
}