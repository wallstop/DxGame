using System.Linq;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;
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
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach(var spatial in DxGame.Instance.DxGameElements.OfType<SpatialComponent>())
            {
                spriteBatch.DrawBorder(spatial.Space, 1, Color.Red);
            }
        }
    }
}