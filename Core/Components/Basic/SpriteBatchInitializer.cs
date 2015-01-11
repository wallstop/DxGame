using System;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    public class SpriteBatchInitializer : DrawableComponent
    {
        public SpriteBatchInitializer(DxGame game)
            : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Rectangle2f screen = DxGame.ScreenRegion;

            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);
            spriteBatch_.Begin(0, null, null, null, null, null, cameraShift);
        }
    }
}