using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace Pong.Core.Components
{
    /**
        <summary>
            Draws a circle of the provided color at the center of the provided spatial
        </summary>
    */

    public class DrawableCircle : DrawableComponent
    {
        public DxCircle Circle => new DxCircle(Spatial.Center, Radius);
        private Color Color { get; }

        private float Radius { get; }

        private PhysicsComponent Spatial { get; }

        public DrawableCircle(PhysicsComponent spatial, Color color, float radius)
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