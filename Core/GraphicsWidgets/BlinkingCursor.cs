using System;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets
{
    public class BlinkingCursor : DrawableComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (BlinkingCursor));

        public TimeSpan BlinkRate { get; protected set; }
        public SpatialComponent SpatialComponent { get; protected set; }
        public bool Drawn { get; protected set; }

        protected TimeSpan LastToggled { get; set; }
        protected Texture2D Texture { get; set; }

        public BlinkingCursor(DxGame game)
            : base(game)
        {
            Drawn = false;
            LastToggled = TimeSpan.FromSeconds(0);
        }

        public BlinkingCursor WithBlinkRate(int blinkRate)
        {
            return WithBlinkRate(TimeSpan.FromSeconds(1.0 / blinkRate));
        }

        public BlinkingCursor WithBlinkRate(TimeSpan blinkRate)
        {
            Debug.Assert(blinkRate > TimeSpan.FromSeconds(0),
                "Blinking Cursor cannot be initialized with a zero timespan!");
            LOG.Debug(String.Format("Setting Blinking Cursor {0} BlinkRate to {1}", this, blinkRate));
            BlinkRate = blinkRate;
            return this;
        }

        public BlinkingCursor WithSpatialComponent(SpatialComponent spatial)
        {
            GenericUtils.CheckNullOrDefault(spatial,
                "Blinking Cursor cannot be instantiaed with a null Spatial Component");
            SpatialComponent = spatial;
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            Texture = new Texture2D(GraphicsDevice, width, height);
            return this;
        }

        public BlinkingCursor WithColor(Color color)
        {
            GenericUtils.CheckNullOrDefault(Texture,
                String.Format("Blinking Cursor cannot be initialized with color {0} without a valid Texture", color));
            LOG.Debug(String.Format("Setting Blinking Cursor {0} Color to {1}", this, color));
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            Texture.SetData(Enumerable.Repeat(color, width * height).ToArray());
            return this;
        }

        public override void Update(GameTime gameTime)
        {
            var totalTime = gameTime.TotalGameTime;
            if (totalTime > LastToggled + BlinkRate)
            {
                // Swap the "Drawn" state if we've exceeded our blink capacity
                Drawn = !Drawn;
                LastToggled = totalTime;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Only draw ourselves if we're in our "Drawn" state
            GenericUtils.CheckNullOrDefault(SpatialComponent);
            GenericUtils.CheckNullOrDefault(Texture);

            if (Drawn)
            {
                spriteBatch_.Draw(Texture, SpatialComponent.Position);
            }
            base.Draw(gameTime);
        }
    }
}