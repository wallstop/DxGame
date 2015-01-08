using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder(Game game) 
            : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch_.End();
        }
    }
}
