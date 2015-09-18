using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
{
    /**

        <summary> Simple developer tool that aims to draw red boxes around every single SpatialComponent attached to the game.</summary>

    */
    public class BoundingBoxWidget : DrawableComponent
    {
        public BoundingBoxWidget(DxGame game) 
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach (var spatial in DxGame.DxGameElements.OfType<SpatialComponent>())
            {
                SpriteBatchUtils.DrawBorder(DxGame, spatial.Space, 1, Color.Red);
            }
        }
    }
}
