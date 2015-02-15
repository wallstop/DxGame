using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.GraphicsWidgets
{
    public class TextBox : DrawableComponent
    {
        public string Text { get; protected set; }
        public SpatialComponent SpatialComponent { get; protected set; }
        public IEnumerable<Keys> ValidKeys { get;
            protected set { value = value.Concat(Keys.Back, Keys.Delete); }; }

        private int cursorPosition_;
        private readonly BlinkingCursor blinkingCursor_;

        protected int CursorPosition
        {
            get { return cursorPosition_; }
            set
            {
                // TODO: Move the bounding logic more into here (will have to change how keypresses are handled - each keystroke will update the string)
                cursorPosition_ = Math.Max(value, 0);
                if (IsMaxLengthSet())
                {
                    cursorPosition_ = MathUtils.Constrain(cursorPosition_, 0, MaxLength - 1);
                }
            }
        }

        protected SpriteFont SpriteFont { get; set; }
        protected Texture2D Texture { get; set; }
        protected Color TextColor { get; set; }
        protected int MaxLength { get; set; }

        protected bool InFocus
        {
            get
            {
                var mouseState = Mouse.GetState();
                var mousePosition = mouseState.Position;
                return SpatialComponent.Space.Contains(mousePosition);
            }
        }

        public TextBox(DxGame game)
            : base(game)
        {
            Text = "";
            CursorPosition = 0;
            MaxLength = 0;
            blinkingCursor_ = new BlinkingCursor(DxGame).WithHeight(1); // 1-pixel blinking cursor by default
            ValidKeys = KeyboardEvent.KeyCharacters.Keys;
        }

        public TextBox WithSpriteFont(SpriteFont spriteFont)
        {
            GenericUtils.CheckNullOrDefault(spriteFont, "Cannot instantiate a TextBox with a null SpriteFont!");
            SpriteFont = spriteFont;

            const string testString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringMeasurement = SpriteFont.MeasureString(testString);
            // Take an average width over the alphabet
            blinkingCursor_.WithWidth(stringMeasurement.X / testString.Length);
            return this;
        }

        public TextBox WithSpatialComponent(SpatialComponent spatialComponent)
        {
            GenericUtils.CheckNullOrDefault(spatialComponent,
                "Cannot instantiate a TextBox with a null Spatial Component!");
            SpatialComponent = spatialComponent;
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            Texture = new Texture2D(GraphicsDevice, width, height);
            return WithBackGroundColor(Color.White);
        }

        public TextBox WithBackGroundColor(Color color)
        {
            GenericUtils.CheckNullOrDefault(Texture,
                String.Format("Cannot initialize a TextBox with color {0} without a valid Texture", color));
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            // Enumerable.Repeat is very slow - might want to use for loop
            Texture.SetData(Enumerable.Repeat(color, width * height).ToArray());
            return this;
        }

        public TextBox WithTextColor(Color color)
        {
            TextColor = color;
            blinkingCursor_.WithColor(color);
            return this;
        }

        public TextBox WithMaxLength(int length)
        {
            Debug.Assert(length > 0, "It's pointless to create a TextBox that can't have any text in it");
            MaxLength = length;
            return this;
        }

        public TextBox WithValidKeys(IEnumerable<Keys> validKeys)
        {
            var validKeysEnumerated = validKeys as Keys[] ?? validKeys.ToArray();
            GenericUtils.CheckNullOrDefault(validKeysEnumerated,
                "Cannot initialize a TextBox with an empty set of valid keys");
            ValidKeys = validKeysEnumerated;
            return this;
        }

        public override void Draw(GameTime gameTime)
        {
            GenericUtils.CheckNullOrDefault(SpatialComponent, "Can't use a TextBox without a spatial component!");
            GenericUtils.CheckNullOrDefault(SpriteFont, "Can't use a TextBox without a sprite font!");
            GenericUtils.CheckNullOrDefault(Texture, "Can't use a TextBox without a Texture!");

            spriteBatch_.Draw(Texture, SpatialComponent.Position);
            spriteBatch_.DrawString(SpriteFont, Text, SpatialComponent.Position, TextColor);

            // TODO: Change this to some kind of focused-style state, possible set via the owner
            if (InFocus)
            {
                blinkingCursor_.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Have this linked to some cursor object instead of directly reading the mouse state

            // Only update if we have focus
            if (InFocus)
            {
                var inputModel = DxGame.Model<InputModel>();
                IEnumerable<KeyboardEvent> finishedKeys =
                    inputModel.FinishedEvents.Where(key => (ValidKeys.Contains(key.Key)));
                HandleKeyboardEvents(finishedKeys);
                IEnumerable<KeyboardEvent> longPressedKeys =
                    inputModel.Events.Where(key => (key.HeldDown && !ValidKeys.Contains(key.Key)));
                HandleKeyboardEvents(longPressedKeys);
            }

            var textSubstring = Text.Substring(0, CursorPosition);
            blinkingCursor_.WithOrigin(new Vector2(
                SpatialComponent.Position.X + SpriteFont.MeasureString(textSubstring).X,
                SpatialComponent.Position.Y +
                // TODO: Find some better way of determining string height when the string is empty than hard-coded "a"
                SpriteFont.MeasureString(textSubstring.Length == 0 ? "a" : textSubstring).Y));
            blinkingCursor_.Update(gameTime);

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
            string preCursor = Text.Substring(0, CursorPosition);
            string postCursor = Text.Substring(CursorPosition);
            string typedText = "";
            foreach (KeyboardEvent keyEvent in events)
            {
                var minCursorPos = Math.Max(0, CursorPosition - 1);
                switch (keyEvent.Key)
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
                    if (KeyboardEvent.KeyCharacters.ContainsKey(keyEvent.Key))
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
            if (IsMaxLengthSet() && MaxLength < Text.Length)
            {
                Text = Text.Substring(0, MaxLength);
            }
        }


        protected bool IsMaxLengthSet()
        {
            return MaxLength != 0;
        }
    }
}