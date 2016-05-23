using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Cache.Simple;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework;
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
        public DxRectangle Entrance { get; }

        [DataMember]
        private Tile[][] TileGrid { get; }

        [IgnoreDataMember]
        [NonSerialized]
        private readonly ISimpleCache<Tile, Texture2D> tileTextureCache_ = new UnboundedLoadingSimpleCache<Tile, Texture2D>(tile => DxGame.Instance.Content.Load<Texture2D>(tile.Asset));

        public static DxRectangle DetermineEntrance(MapDescriptor descriptor)
        {
            // TODO
            return DxRectangle.EmptyRectangle;
            //var boundary = ScaledBounds;
            //DxRectangle spawnLocation;
            //do
            //{
            //    /* Pick a random place that has at least a 500x500 area of "no map collidables" */
            //    /* TODO: Have this be some kind of chooseable algorithm or something (bundled in map descriptor? ) */
            //    spawnLocation =
            //        new DxRectangle(
            //            ThreadLocalRandom.Current.NextFloat(boundary.X, boundary.X + boundary.Width - 250),
            //            ThreadLocalRandom.Current.NextFloat(boundary.Y, boundary.Y + boundary.Height - 250), 250,
            //            250);
            //} while(CollidesWithMap(spawnLocation));
            //return spawnLocation;
        }

        public static Tile[][] DetermineTileGrid(MapDescriptor mapDescriptor)
        {
            Validate.IsNotNullOrDefault(mapDescriptor);
            Tile [][] tileGrid = new Tile[mapDescriptor.Width][];
            for(int i = 0; i < mapDescriptor.Width; ++i)
            {
                tileGrid[i] = new Tile[mapDescriptor.Height];
            }

            foreach(KeyValuePair<TilePosition, Tile> tilePair in mapDescriptor.Tiles)
            {
                TilePosition position = tilePair.Key;
                Tile tile = tilePair.Value;
                tileGrid[position.X][position.Y] = tile;
            }
            return tileGrid;
        }

        public Map(MapDescriptor descriptor)
        {
            Validate.IsNotNullOrDefault(descriptor, this.GetFormattedNullOrDefaultMessage(nameof(descriptor)));
            MapDescriptor = descriptor;
            Entrance = DetermineEntrance(descriptor);
            TileGrid = DetermineTileGrid(descriptor);
            DrawPriority = DrawPriority.MAP;
        }

        public override void LoadContent()
        {
            /* TODO: If loading is too slow, determine unique tiles in DetermineTileGrid and pass them around */

            foreach(Tile[] tileColumn in TileGrid)
            {
                foreach(Tile tile in tileColumn)
                {
                    /* Loading cache - textures should be loaded in automagically */
                    tileTextureCache_.Get(tile);
                }
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
            /* 
                Map is pretty special: We want actually want to draw whatever chunk of the map the screen is currently seeing. 
                However, we've already set up a matrix translation on our camera for every object. So, we need to undo the translation (just flip x & y)
            */
            DxRectangle target = range;
            target.X = -target.X;
            target.Y = -target.Y;
            foreach(var mapLayerEntry in MapTexturesAndLayers)
            {
                Texture2D mapImage = mapLayerEntry.Key;
                int layer = mapLayerEntry.Value;
                spriteBatch.Draw(mapImage, target.ToRectangle(), (target / MapDescriptor.Scale).ToRectangle(),
                    Color.White, 0, new Vector2(), SpriteEffects.None, layer);
            }
        }

        private bool CollidesWithMap(DxRectangle region)
        {
            List<MapCollidableComponent> collisions = Collidables.InRange(region);
            return collisions.Any(collidable => collidable.Spatial.Space.Intersects(region));
        }
    }
}