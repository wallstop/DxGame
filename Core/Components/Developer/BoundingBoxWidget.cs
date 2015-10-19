using System.Linq;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
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
            foreach (var spatial in DxGame.Instance.DxGameElements.OfType<SpatialComponent>())
            {
                spriteBatch.DrawBorder(spatial.Space, 1, Color.Red);
            }
        }
    }
}