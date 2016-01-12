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

        [NonSerialized] [IgnoreDataMember] private readonly Dictionary<Texture2D, int> mapTexturesAndLayers_ =
            new Dictionary<Texture2D, int>();

        [DataMember]
        public MapDescriptor MapDescriptor { get; private set; }

        [IgnoreDataMember]
        public DxRectangle ScaledBounds => MapDescriptor.Size * MapDescriptor.Scale;

        [IgnoreDataMember]
        public Dictionary<Texture2D, int> MapTexturesAndLayers => mapTexturesAndLayers_;

        [DataMember]
        public ISpatialTree<MapCollidableComponent> Collidables { get; private set; }

        [DataMember]
        public DxVector2 PlayerSpawn { get; private set; }

        public DxRectangle RandomSpawnLocation
        {
            get
            {
                var boundary = ScaledBounds;
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

        public Map(MapDescriptor descriptor)
        {
            Validate.IsNotNullOrDefault(descriptor,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor)));
            MapDescriptor = descriptor;
            DrawPriority = DrawPriority.MAP;
        }

        public override void LoadContent()
        {
            mapTexturesAndLayers_.Clear();
            if(!MapDescriptor.MapLayers.Any())
            {
                LOG.Info($"Attempted to {nameof(LoadContent)} for a null/empty Asset, defaulting to blank texture");
                Texture2D simpleBlackBackground = TextureFactory.TextureForColor(Color.Black);
                mapTexturesAndLayers_[simpleBlackBackground] = MapLayer.DEFAULT_LAYER;
            }
            else
            {
                foreach(MapLayer mapLayer in MapDescriptor.MapLayers)
                {
                    Texture2D mapImage =
                        DxGame.Instance.Content.Load<Texture2D>("Map/" +
                                                                Path.GetFileNameWithoutExtension(mapLayer.Asset));
                    mapTexturesAndLayers_[mapImage] = mapLayer.Layer;
                }
            }
            base.LoadContent();
        }

        public override void Initialize()
        {
            List<MapCollidableComponent> mapSpatials = MapDescriptor.Platforms.Select(platform =>
            {
                var spatial =
                    (SpatialComponent)
                        SpatialComponent.Builder()
                            .WithDimensions(new DxVector2(platform.BoundingBox.Width, platform.BoundingBox.Height))
                            .WithPosition(new DxVector2(platform.BoundingBox.XY()))
                            .Build();
                return
                    (MapCollidableComponent)
                        MapCollidableComponent.Builder()
                            .WithPlatformType(platform.Type)
                            .WithCollidableDirections(platform.CollidableDirections)
                            .WithSpatial(spatial)
                            .Build();
            }).ToList();

            Collidables = new RTree<MapCollidableComponent>(spatial => spatial.Spatial.Space, mapSpatials);
            PlayerSpawn = new DxVector2(RandomSpawnLocation.XY());
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