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

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class Map : DrawableComponent
    {
        [NonSerialized] [IgnoreDataMember] private Texture2D mapTexture_;

        [DataMember]
        public MapDescriptor MapDescriptor { get; private set; }

        [IgnoreDataMember]
        public Texture2D MapTexture
        {
            get { return mapTexture_; }
            private set { mapTexture_ = value; }
        }

        [DataMember]
        public ICollisionTree<MapCollidableComponent> Collidables { get; private set; }

        [DataMember]
        public DxVector2 PlayerSpawn { get; private set; }

        public DxRectangle RandomSpawnLocation
        {
            get
            {
                var boundary = MapDescriptor.Size * MapDescriptor.Scale;
                DxRectangle spawnLocation;
                do
                {
                    /* Pick a random place that has at least a 500x500 area of "no map collidables" */
                    /* TODO: Have this be some kind of chooseable algorithm or something (bundled in map descriptor? ) */
                    spawnLocation =
                        new DxRectangle(
                            ThreadLocalRandom.Current.NextFloat(boundary.X, (boundary.X + boundary.Width)),
                            ThreadLocalRandom.Current.NextFloat(boundary.Y, (boundary.Y + boundary.Height)), 250, 250);
                } while (CollidesWithMap(spawnLocation));
                return spawnLocation;
            }
        }

        public Map(DxGame game, MapDescriptor descriptor)
            : base(game)
        {
            Validate.IsNotNullOrDefault(descriptor,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor)));
            MapDescriptor = descriptor;
            DrawPriority = DrawPriority.MAP;
        }

        public override void LoadContent()
        {
            MapTexture = DxGame.Content.Load<Texture2D>("Map/" + Path.GetFileNameWithoutExtension(MapDescriptor.Asset));
            base.LoadContent();
        }

        public override void Initialize()
        {
            List<MapCollidableComponent> mapSpatials =
                MapDescriptor.Platforms.Select(
                    platform =>
                    {
                        var spatial =
                            (SpatialComponent)
                                SpatialComponent.Builder().WithDimensions(new DxVector2(platform.BoundingBox.Width,
                                    platform.BoundingBox.Height))
                                    .WithPosition(new DxVector2(platform.BoundingBox.XY()))
                                    .Build();
                        return (MapCollidableComponent)
                            MapCollidableComponent.Builder().WithPlatformType(platform.Type).
                                WithCollidableDirections(platform.CollidableDirections).WithSpatial(spatial).Build();
                    })
                    .ToList();

            Collidables = new RTree<MapCollidableComponent>((spatial => spatial.Spatial.Space), mapSpatials);
            PlayerSpawn = new DxVector2(RandomSpawnLocation.XY());
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var range = DxGame.ScreenRegion;
            /* 
                Map is pretty special: We want actually want to draw whatever chunk of the map the screen is currently seeing. 
                However, we've already set up a matrix translation on our camera for every object. So, we need to undo the translation (just flip x & y)
            */
            var target = range;
            target.X = -target.X;
            target.Y = -target.Y;
            spriteBatch.Draw(MapTexture, (target).ToRectangle(), (target / MapDescriptor.Scale).ToRectangle(),
                Color.White);
        }

        private bool CollidesWithMap(DxRectangle region)
        {
            List<MapCollidableComponent> collisions = Collidables.InRange(region);
            return collisions.Any(collidable => collidable.Spatial.Space.Intersects(region));
        }
    }
}