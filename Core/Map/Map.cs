using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Map
{
    public class Map : DrawableComponent
    {
        public MapDescriptor MapDescriptor { get; }
        public Texture2D MapTexture { get; private set; }
        public QuadTree<CollidableComponent> Collidables { get; private set; }
        public DxVector2 PlayerSpawn { get; private set; }

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
            List<CollidableComponent> mapSpatials =
                MapDescriptor.Platforms.Select(
                    platform =>
                        (CollidableComponent)
                            new CollidableComponent(DxGame).WithCollidableDirections(platform.CollidableDirections)
                                .WithDimensions(new DxVector2(platform.BoundingBox.Width,
                                    platform.BoundingBox.Height)).WithPosition(new DxVector2(platform.BoundingBox.XY())))
                    .ToList();

            DxGame.AddAndInitializeComponents(mapSpatials);

            Collidables = new QuadTree<CollidableComponent>((spatial => spatial.Center), MapDescriptor.Size,
                mapSpatials);
            DeterminePlayerSpawn();
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
            spriteBatch.Draw(MapTexture, target.ToRectangle(), target.ToRectangle(), Color.White);
        }

        public void DeterminePlayerSpawn()
        {
            var boundary = MapDescriptor.Size;
            var rGen = new Random();
            DxRectangle playerSpawn;
            do
            {
                playerSpawn = new DxRectangle(rGen.Next((int) boundary.X, (int) (boundary.X + boundary.Width)),
                    rGen.Next((int) boundary.Y, (int) (boundary.Y + boundary.Height)), 300, 300);
            } while (CollidesWithMap(playerSpawn));
            PlayerSpawn = new DxVector2(playerSpawn.XY());
        }

        private bool CollidesWithMap(DxRectangle region)
        {
            List<CollidableComponent> collisions = Collidables.InRange(region);
            return collisions.Any(collidable => collidable.Space.Intersects(region));
        }
    }
}