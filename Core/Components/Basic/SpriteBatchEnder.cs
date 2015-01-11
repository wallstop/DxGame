using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    internal class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.END_SPRITEBATCH;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch_.End();
        }
    }
}