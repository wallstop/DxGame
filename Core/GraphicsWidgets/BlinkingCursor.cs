using System;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets
{
    /**
        <summary> It's a cursor! It blinks! Might be a texture, might be a little rectangle, who knows! </summary>
    */
    public class BlinkingCursor : DrawableComponent
    {
        public DxRectangle Space { get; set; }

        public TimeSpan BlinkRate { get; }
        protected Texture2D Texture { get; }

        public bool Drawn { get; protected set; } = true;
        protected TimeSpan LastToggled { get; set; } = TimeSpan.Zero;
        public override bool ShouldSerialize => false;

        private BlinkingCursor(DxGame game, Texture2D cursorTexture, TimeSpan blinkRate, DxRectangle space)
            : base(game)
        {
            Space = space;
            BlinkRate = blinkRate;
            Texture = cursorTexture;
        }

        public static BlinkingCursorBuilder Builder()
        {
            return new BlinkingCursorBuilder();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var totalTime = gameTime.TotalGameTime;
            if (LastToggled + BlinkRate <= totalTime)
            {
                // Swap the "Drawn" state if we've exceeded our blink capacity
                Drawn = !Drawn;
                LastToggled = totalTime;
            }
            if (Drawn)
            {
                spriteBatch.Draw(Texture, destinationRectangle: Space.ToRectangle());
            }
        }

        public class BlinkingCursorBuilder : IBuilder<BlinkingCursor>
        {
            private TimeSpan blinkRate_ = TimeSpan.FromSeconds(1.0 / 2);
            private Texture2D cursorTexture_ = TextureFactory.TextureForColor(Color.Black);
            private DxGame game_;
            /* Default to origin, 1 pixel wide, 1 pixel tall */
            private DxRectangle space_ = new DxRectangle(0, 0, 1, 1);

            public BlinkingCursor Build()
            {
                Validate.IsNotNullOrDefault(cursorTexture_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "CursorTexture"));
                Validate.IsTrue(blinkRate_ > TimeSpan.Zero,
                    $"Cannot create a {typeof (BlinkingCursor)} with a BlinkRate of {blinkRate_}");
                Validate.IsTrue(space_.Height > 0, $"Cannot create a {typeof(BlinkingCursor)} with a negative height ({space_.Height})!");
                Validate.IsTrue(space_.Width > 0, $"Cannot create a {typeof(BlinkingCursor)} with a negative width ({space_.Width})!");
                if (Check.IsNullOrDefault(game_))
                {
                    game_ = DxGame.Instance;
                }
                /* Space can always be set (and will probably be updated) post-construction, so it's ok to have a default rectangle space */
                return new BlinkingCursor(game_, cursorTexture_, blinkRate_, space_);
            }

            public BlinkingCursorBuilder WithGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public BlinkingCursorBuilder WithBlinkRate(TimeSpan blinkRate)
            {
                blinkRate_ = blinkRate;
                return this;
            }

            /* 
                Floating-point width allows us to do precise calcultions of where a cursor should be. For example, in a text box, a font might not be monospaced 
                (or it might be monospaced, but it's spacing doesn't align perfectly to pixels). This floating point offset allows us to do math like
                cursorPosition = index * Cursor.Width kind of thing 
            */

            public BlinkingCursorBuilder WithWidth(float width)
            {
                space_.Width = width;
                return this;
            }

            public BlinkingCursorBuilder WithHeight(float height)
            {
                space_.Height = height;
                return this;
            }

            public BlinkingCursorBuilder WithOrigin(Vector2 origin)
            {
                space_.X = (int) origin.X;
                space_.Y = (int) origin.Y;
                return this;
            }

            public BlinkingCursorBuilder WithSpace(DxRectangle space)
            {
                space_ = space;
                return this;
            }

            public BlinkingCursorBuilder WithTexture(Texture2D texture)
            {
                cursorTexture_ = texture;
                return this;
            }

            public BlinkingCursorBuilder WithColor(Color color)
            {
                cursorTexture_ = TextureFactory.TextureForColor(color);
                return this;
            }
        }
    }
}