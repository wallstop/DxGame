using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced.Position;
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
        public QuadTree<SpatialComponent> Collidables { get; private set; }

        public Map(DxGame game, MapDescriptor descriptor)
            : base(game)
        {
            Validate.IsNotNullOrDefault(descriptor,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor)));
            MapDescriptor = descriptor;
        }

        public override void LoadContent()
        {
            MapTexture = DxGame.Content.Load<Texture2D>(MapDescriptor.Asset);
            base.LoadContent();
        }

        public override void Initialize()
        {
            List<SpatialComponent> mapSpatials =
                MapDescriptor.Platforms.Select(
                    platform =>
                        (SpatialComponent)
                            new SpatialComponent(DxGame).WithDimensions(new DxVector2(platform.BoundingBox.Width,
                                platform.BoundingBox.Height)).WithPosition(new DxVector2(platform.BoundingBox.XY())))
                    .ToList();

            Collidables = new QuadTree<SpatialComponent>((spatial => spatial.Dimensions), MapDescriptor.Size,
                mapSpatials);
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var range = DxGame.ScreenRegion;
            spriteBatch.Draw(MapTexture, range.ToRectangle(), range.ToRectangle(), Color.White);
        }
    }
}