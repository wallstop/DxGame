using System;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public class FpsWidget : DrawableComponent
    {
        private static readonly int PIXEL_OFFSET = 5;
        private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromSeconds(1.0 / 10);
        private TimeSpan lastUpdated_ = TimeSpan.Zero;

        private readonly FpsTracker fpsTracker_;
        private SpriteFont spriteFont_;
        private string fps_;

        public FpsWidget(DxGame game) 
            : base(game)
        {
            fpsTracker_ = new FpsTracker(game);
        }

        public override void Initialize()
        {
            fpsTracker_.Initialize();
            base.Initialize();
        }

        public override void LoadContent()
        {
            spriteFont_ = DxGame.Content.Load<SpriteFont>("Fonts/Pericles");
            fpsTracker_.LoadContent();
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            fpsTracker_.Process(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            fpsTracker_.Draw(spriteBatch, gameTime);
            if (lastUpdated_ + UPDATE_INTERVAL < gameTime.TotalGameTime)
            {
                fps_ = fpsTracker_.FramesPerSecond.ToString();
                lastUpdated_ = gameTime.TotalGameTime;
            }

            var size = spriteFont_.MeasureString(fps_);
            var screenRegion = DxGame.ScreenRegion;
            // TODO: Fix whatever weird math is being done with the screen region to make drawing things "sane"
            var drawLocation = new Vector2(Math.Abs(screenRegion.X) + screenRegion.Width - size.X - PIXEL_OFFSET, Math.Abs(screenRegion.Y) + PIXEL_OFFSET);
            spriteBatch.DrawString(spriteFont_, fps_, drawLocation, Color.White);
        }
    }
}
