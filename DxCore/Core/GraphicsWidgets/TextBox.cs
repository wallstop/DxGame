using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.GraphicsWidgets
{
    public class TextBox : DrawableComponent
    {
        private int cursorPosition_;
        private ImmutableHashSet<Keys> validKeys_ = ImmutableHashSet<Keys>.Empty;
        public string Text { get; protected set; }
        public SpatialComponent SpatialComponent { get; }

        public IEnumerable<Keys> ValidKeys
        {
            get { return validKeys_; }
            // Make sure we can use the arrow keys to move, regardless of input
            protected set
            {
                validKeys_ = value.Concat(new[] {Keys.Back, Keys.Delete, Keys.Left, Keys.Right}).ToImmutableHashSet();
            }
        }

        protected int CursorPosition
        {
            get { return cursorPosition_; }
            set
            {
                // TODO: Move the bounding logic more into here (will have to change how keypresses are handled - each keystroke will update the string)
                cursorPosition_ = Math.Max(value, 0);
                if(IsMaxLengthSet())
                {
                    cursorPosition_ = MathHelper.Clamp(cursorPosition_, 0, MaxLength);
                }
            }
        }

        public override bool ShouldSerialize => false;
        protected BlinkingCursor BlinkingCursor { get; }
        protected SpriteFont SpriteFont { get; }
        protected Texture2D Background { get; }
        protected Color TextColor { get; }
        protected int MaxLength { get; }

        protected bool InFocus
        {
            get
            {
                var mouseState = Mouse.GetState();
                var mousePosition = mouseState.Position;
                return SpatialComponent.Space.Contains(mousePosition);
            }
        }

        private TextBox(BlinkingCursor blinkingCursor, List<Keys> validKeys, Texture2D background, Color textColor,
            SpatialComponent spatial, SpriteFont spriteFont, int maxLength)
        {
            Text = "";
            CursorPosition = 0;
            SpatialComponent = spatial;
            SpriteFont = spriteFont;
            MaxLength = maxLength;
            ValidKeys = validKeys;
            TextColor = textColor;
            Background = background;
            BlinkingCursor = blinkingCursor;
        }

        public static TextBoxBuilder Builder()
        {
            return new TextBoxBuilder();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(Background, destinationRectangle: SpatialComponent.Space.ToRectangle());
            spriteBatch.DrawString(SpriteFont, Text, SpatialComponent.Position.ToVector2(), TextColor);

            // TODO: Change this to some kind of focused-style state, possible set via the owner
            if(InFocus)
            {
                BlinkingCursor.Draw(spriteBatch, gameTime);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            // TODO: Have this linked to some cursor object instead of directly reading the mouse state

            // Only update if we have focus
            if(InFocus)
            {
                var inputModel = DxGame.Instance.Model<InputModel>();
                IEnumerable<KeyboardEvent> finishedKeys =
                    inputModel.FinishedEvents.Where(key => ValidKeys.Contains(key.Key));
                HandleKeyboardEvents(finishedKeys);
                IEnumerable<KeyboardEvent> longPressedKeys =
                    inputModel.Events.Where(key => key.HeldDown && ValidKeys.Contains(key.Key));
                HandleKeyboardEvents(longPressedKeys);
            }

            var textSubstring = Text.Substring(0, CursorPosition);
            var cursorPosition = BlinkingCursor.Space;

            var textDimensions = SpriteFont.MeasureString(textSubstring);

            cursorPosition.X = SpatialComponent.Position.X + textDimensions.X;
            /* TODO: Find a better way of doing this than hard-coding "a" */
            cursorPosition.Y = SpatialComponent.Position.Y +
                               (textDimensions.Y == 0 ? SpriteFont.MeasureString("a").Y : textDimensions.Y);
            BlinkingCursor.Space = cursorPosition;
            BlinkingCursor.Process(gameTime);
            base.Update(gameTime);
        }

        /*
            For all keys in the given input, checks to see if:
            -Valid character 
                OR
            -Left/Right Key

            And either updates the position of the cursor in the text box (not currently drawn)
                OR
            Types the text from the keys. Note: If multiple keys are down / long pressed,
            you will get shit tons of characters.
        */

        protected void HandleKeyboardEvents(IEnumerable<KeyboardEvent> events)
        {
            // TODO: Stringbuilder?
            string preCursor = Text.Substring(0, CursorPosition);
            string postCursor = Text.Substring(CursorPosition);
            string typedText = "";
            foreach(KeyboardEvent keyEvent in events)
            {
                var minCursorPos = Math.Max(0, CursorPosition - 1);
                switch(keyEvent.Key)
                {
                    case Keys.Left:
                        CursorPosition = minCursorPos;
                        break;
                    case Keys.Right:
                        // Bound to length of word
                        CursorPosition = Math.Min(CursorPosition + 1, Text.Length);
                        break;
                    case Keys.Back: // Backspace - delete 1 char (to a max of the front of the word)
                        preCursor = preCursor.Substring(0, minCursorPos);
                        CursorPosition = minCursorPos;
                        break;
                    case Keys.Delete: // Delete - delete 1 char to a max of the back of the word
                        postCursor = postCursor.Substring(Math.Min(1, postCursor.Length));
                        break;
                    default:
                        if(KeyboardEvent.KeyCharacters.ContainsKey(keyEvent.Key))
                        {
                            // If this is slow, use a StringBuilder (don't care enough right now)
                            // The length of the word has increased - ok to do unchecked ++CursorPosition
                            typedText += KeyboardEvent.KeyCharacters[keyEvent.Key];
                            ++CursorPosition;
                        }
                        break;
                }
            }

            Text = preCursor + typedText + postCursor;
            if(IsMaxLengthSet() && MaxLength < Text.Length)
            {
                Text = Text.Substring(0, MaxLength);
            }
        }

        protected bool IsMaxLengthSet()
        {
            return MaxLength != 0;
        }

        public class TextBoxBuilder : IBuilder<TextBox>
        {
            private Texture2D backgroundTexture_ = TextureFactory.TextureForColor(Color.White);
            private Color cursorColor_ = Color.Black;
            private int maxLength_;
            private SpatialComponent spatial_;
            private SpriteFont spriteFont_;
            private Color textColor_ = Color.Black;
            private List<Keys> validKeys_ = new List<Keys>(KeyboardEvent.KeyCharacters.Keys);

            public TextBox Build()
            {
                Validate.IsNotNullOrDefault(spriteFont_, this.GetFormattedNullOrDefaultMessage(spriteFont_));
                Validate.IsTrue(maxLength_ > 0, $"Cannot create a {typeof(TextBox)} with a MaxLength of {maxLength_}");
                Validate.IsNotNullOrDefault(spatial_, this.GetFormattedNullOrDefaultMessage(spatial_));

                const string testString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                var stringMeasurement = spriteFont_.MeasureString(testString);
                var textWidth = stringMeasurement.X / testString.Length;
                var blinkingCursor = BlinkingCursor.Builder().WithWidth(textWidth).WithColor(cursorColor_).Build();

                return new TextBox(blinkingCursor, validKeys_, backgroundTexture_, textColor_, spatial_, spriteFont_,
                    maxLength_);
            }

            public TextBoxBuilder WithValidKeys(IEnumerable<Keys> validKeys)
            {
                validKeys_ = new List<Keys>(validKeys);
                return this;
            }

            public TextBoxBuilder WithCursorColor(Color color)
            {
                cursorColor_ = color;
                return this;
            }

            public TextBoxBuilder WithBackgroundTexture(Texture2D texture)
            {
                backgroundTexture_ = texture;
                return this;
            }

            public TextBoxBuilder WithBackgroundColor(Color color)
            {
                backgroundTexture_ = TextureFactory.TextureForColor(color);
                return this;
            }

            public TextBoxBuilder WithTextColor(Color color)
            {
                textColor_ = color;
                return this;
            }

            public TextBoxBuilder WithSpatialComponent(SpatialComponent spatial)
            {
                spatial_ = spatial;
                return this;
            }

            public TextBoxBuilder WithSpriteFont(SpriteFont spriteFont)
            {
                spriteFont_ = spriteFont;
                return this;
            }

            public TextBoxBuilder WithMaxLength(int maxLength)
            {
                maxLength_ = maxLength;
                return this;
            }
        }
    }
}