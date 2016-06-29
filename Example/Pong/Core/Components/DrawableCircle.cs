using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong.Core.Components
{
    /**
        <summary>
            Draws a circle of the provided color at the center of the provided spatial
        </summary>
    */
    public class DrawableCircle : DrawableComponent
    {
        private Color Color { get; }

        public DxCircle Circle => new DxCircle(Spatial.Center, Radius);

        private float Radius { get; }

        private SpatialComponent Spatial { get; }

        public DrawableCircle(SpatialComponent spatial, Color color, float radius)
        {
            Validate.Hard.IsNotNullOrDefault(spatial, () => this.GetFormattedNullOrDefaultMessage(spatial));
            Spatial = spatial;
            Validate.Hard.IsPositive(radius, () => this.GetFormattedNullOrDefaultMessage(nameof(radius)));
            Color = color;
            Validate.Hard.IsNotNullOrDefault(color, () => this.GetFormattedNullOrDefaultMessage(color));
            Radius = radius;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.DrawCircle(Circle, Color);
        }
    }
}
