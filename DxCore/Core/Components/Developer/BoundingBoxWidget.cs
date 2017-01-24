using System.Linq;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary> 
            Simple developer tool that aims to draw red boxes around every single SpatialComponent attached to the game.
        </summary>
    */

    public class BoundingBoxWidget : DrawableComponent
    {
        public BoundingBoxWidget(DrawPriority drawPriority = DrawPriority.Low) : base(drawPriority) {}

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach(ISpatial spatial in DxGame.Instance.DxGameElements.OfType<ISpatial>())
            {
                spriteBatch.DrawBorder(spatial.Space, 1, Color.Red);
            }
        }
    }
}