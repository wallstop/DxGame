using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    public class SpriteBatchInitializer : DrawableComponent
    {
        public SpriteBatchInitializer(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.INIT_SPRITEBATCH;
        }

        public override void Draw(DxGameTime gameTime)
        {
            DxGame.GraphicsDevice.Clear(Color.CornflowerBlue);

            DxRectangle screen = DxGame.ScreenRegion;

            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);
            spriteBatch_.Begin(0, null, null, null, null, null, cameraShift);
        }
    }
}