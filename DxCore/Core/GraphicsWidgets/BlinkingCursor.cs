using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.GraphicsWidgets
{
    /**
        <summary> It's a cursor! It blinks! Might be a texture, might be a little rectangle, who knows! </summary>
    */

    public class BlinkingCursor : DrawableComponent
    {
        public TimeSpan BlinkRate { get; }

        public bool Drawn { get; protected set; } = true;
        public DxRectangle Space { get; set; }
        protected TimeSpan LastToggled { get; set; } = TimeSpan.Zero;
        protected Texture2D Texture { get; }

        private BlinkingCursor(Texture2D cursorTexture, TimeSpan blinkRate, DxRectangle space)
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
            if(LastToggled + BlinkRate <= totalTime)
            {
                // Swap the "Drawn" state if we've exceeded our blink capacity
                Drawn = !Drawn;
                LastToggled = totalTime;
            }
            if(Drawn)
            {
                spriteBatch.Draw(Texture, destinationRectangle: Space.ToRectangle());
            }
        }

        public class BlinkingCursorBuilder : IBuilder<BlinkingCursor>
        {
            private TimeSpan blinkRate_ = TimeSpan.FromSeconds(1.0 / 2);
            private Texture2D cursorTexture_ = TextureFactory.TextureForColor(Color.Black);
            /* Default to origin, 1 pixel wide, 1 pixel tall */
            private DxRectangle space_ = new DxRectangle(0, 0, 1, 1);

            public BlinkingCursor Build()
            {
                Validate.Hard.IsNotNullOrDefault(cursorTexture_, this.GetFormattedNullOrDefaultMessage("CursorTexture"));
                Validate.Hard.IsTrue(blinkRate_ > TimeSpan.Zero,
                    $"Cannot create a {typeof(BlinkingCursor)} with a BlinkRate of {blinkRate_}");
                Validate.Hard.IsTrue(space_.Height > 0,
                    $"Cannot create a {typeof(BlinkingCursor)} with a negative height ({space_.Height})!");
                Validate.Hard.IsTrue(space_.Width > 0,
                    $"Cannot create a {typeof(BlinkingCursor)} with a negative width ({space_.Width})!");
                /* Space can always be set (and will probably be updated) post-construction, so it's ok to have a default rectangle space */
                return new BlinkingCursor(cursorTexture_, blinkRate_, space_);
            }

            public BlinkingCursorBuilder WithBlinkRate(TimeSpan blinkRate)
            {
                blinkRate_ = blinkRate;
                return this;
            }

            public BlinkingCursorBuilder WithColor(Color color)
            {
                cursorTexture_ = TextureFactory.TextureForColor(color);
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
        }
    }
}