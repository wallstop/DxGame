using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.GraphicsWidgets.HUD
{
    /**
        <summary>
            Calculates and draws the current FPS of the game in the upper right-hand corner of the screen
        </summary>
    */
    public class FpsWidget : DrawableComponent
    {
        /* X and Y offset from upper-right corner to draw this widget */
        private static readonly int PIXEL_OFFSET = 5;
        private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromSeconds(1.0 / 10);
        private TimeSpan lastUpdated_ = TimeSpan.Zero;

        private readonly FpsTracker fpsTracker_;
        private SpriteFont spriteFont_;
        private string fps_;

        public FpsWidget()
        {
            fpsTracker_ = new FpsTracker();
        }

        public override void Initialize()
        {
            fpsTracker_.Initialize();
            base.Initialize();
        }

        public override void LoadContent()
        {
            spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles_20");
            fpsTracker_.LoadContent();
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            fpsTracker_.Process(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            fpsTracker_.Draw(spriteBatch, gameTime);
            if (lastUpdated_ + UPDATE_INTERVAL < gameTime.TotalGameTime)
            {
                fps_ = $"FPS: {fpsTracker_.FramesPerSecond}";
                lastUpdated_ = gameTime.TotalGameTime;
            }

            var size = spriteFont_.MeasureString(fps_);
            var screenRegion = DxGame.Instance.ScreenRegion;
            // TODO: Fix whatever weird math is being done with the screen region to make drawing things "sane"
            var drawLocation = new Vector2(Math.Abs(screenRegion.X) + screenRegion.Width - size.X - PIXEL_OFFSET, Math.Abs(screenRegion.Y) + PIXEL_OFFSET);
            const float transparencyWeight = 0.8f;
            var transparency = ColorFactory.Transparency(transparencyWeight);
            var blackTexture = TextureFactory.TextureForColor(Color.Black);
            spriteBatch.Draw(blackTexture, color: transparency, destinationRectangle: new DxRectangle(drawLocation.X, drawLocation.Y, size.X, size.Y).ToRectangle());
            spriteBatch.DrawString(spriteFont_, fps_, drawLocation, Color.White);
        }
    }
}
