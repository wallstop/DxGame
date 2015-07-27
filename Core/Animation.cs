using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core
{
    [Serializable]
    [DataContract]
    public class Animation
    {
        /* TODO: Configure FPS for animations based on Holden's animations. Make it configurable? */
        private static readonly double ANIMATION_FRAMES_PER_SECOND = 60;
        [DataMember] private readonly int totalFrames_;
        [DataMember] private int currentFrame_;
        [DataMember] private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);
        [DataMember] protected PositionalComponent position_;
        [NonSerialized] [IgnoreDataMember] private Texture2D spriteSheet_;
        public TimeSpan TimePerFrame => TimeSpan.FromSeconds(1.0f / ANIMATION_FRAMES_PER_SECOND);

        [DataMember]
        public string AssetName { get; set; }

        /* TODO: Remove default values for width & height */

        /* We asume that Animations are horizontal strips without any spacing or anything between frames */

        public Animation(string spriteSheet, int totalFrames = 1)
        {
            Validate.IsTrue(totalFrames >= 1,
                $"Cannot initialize an {nameof(Animation)} with {nameof(totalFrames)} of {totalFrames}");
            Validate.IsNotNullOrDefault(spriteSheet,
                StringUtils.GetFormattedNullDefaultMessage(this, nameof(spriteSheet)));
            AssetName = spriteSheet;
            totalFrames_ = totalFrames;
        }

        /* TODO: Make proper builders for everything */

        public Animation WithPosition(PositionalComponent position)
        {
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullDefaultMessage(this, position));
            position_ = position;
            return this;
        }

        /* TODO: make this actually handle spritesheets and animations (LOL) */
        /* Bundle Update + Draw into one, might want to break this out into two separate functions later */

        public void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // Assume gameTime is non-null (if it is we have some issues)
            int frameWidth = spriteSheet_.Width / totalFrames_;
            Rectangle frameOutline = new Rectangle(frameWidth * currentFrame_, 0, frameWidth, spriteSheet_.Height);
            spriteBatch.Draw(spriteSheet_, position_.Position.ToVector2(), frameOutline, Color.White, 0.0f, Vector2.Zero,
                1.0f,
                SpriteEffects.None, 0);
        }

        private void UpdateCurrentFrame(DxGameTime gameTime)
        {
            if (gameTime.TotalGameTime >= (lastUpdated_ + TimePerFrame))
            {
                lastUpdated_ = gameTime.TotalGameTime;
                currentFrame_ = MathUtils.WrappedAdd(currentFrame_, 1, totalFrames_);
            }
        }

        public void Reset()
        {
            currentFrame_ = 0;
        }

        public bool LoadContent(ContentManager contentManager)
        {
            spriteSheet_ = contentManager.Load<Texture2D>(AssetName);
            return true;
        }
    }
}