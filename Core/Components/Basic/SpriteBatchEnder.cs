using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Basic
{
    public class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.END_SPRITEBATCH;
        }

        public override void Draw(DxGameTime gameTime)
        {
            spriteBatch_.End();
        }
    }
}