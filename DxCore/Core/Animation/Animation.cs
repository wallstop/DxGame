using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Animation
{
    [Serializable]
    [DataContract]
    public class Animation
    {
        [DataMember] private int currentFrame_;
        [DataMember] private DrawPriority drawPriority_;
        [DataMember] private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);

        [NonSerialized] [IgnoreDataMember] private Texture2D spriteSheet_;

        [DataMember]
        public AnimationDescriptor AnimationDescriptor { get; private set; }

        public DrawPriority DrawPriority => drawPriority_;

        public TimeSpan TimePerFrame => TimeSpan.FromSeconds(1.0f / AnimationDescriptor.FramesPerSecond);

        public UpdatePriority UpdatePriority => UpdatePriority.Normal;
        private float Scale => (float) AnimationDescriptor.Scale;

        private int TotalFrames => AnimationDescriptor.FrameCount;

        public Animation(AnimationDescriptor descriptor, DrawPriority drawPriority = DrawPriority.Normal)
        {
            Validate.Hard.IsNotNullOrDefault(descriptor, () => this.GetFormattedNullOrDefaultMessage(descriptor));
            Validate.Hard.IsPositive(descriptor.FrameCount,
                () => $"Cannot initialize an {nameof(Animation)} with a FrameCount of {descriptor.FrameCount}");
            Validate.Hard.IsPositive(descriptor.FramesPerSecond,
                () =>
                        $"Cannot initialize an {nameof(Animation)} with a {nameof(descriptor.FramesPerSecond)} of {descriptor.FramesPerSecond}");
            Validate.Hard.IsNotNullOrDefault(descriptor.Asset,
                () => this.GetFormattedNullOrDefaultMessage(nameof(descriptor.Asset)));
            AnimationDescriptor = descriptor;
            drawPriority_ = drawPriority;
        }

        public void Draw(SpriteBatch spriteBatch, DxGameTime gameTime, DxVector2 position, Direction orientation)
        {
            while(lastUpdated_ + TimePerFrame < gameTime.TotalGameTime)
            {
                lastUpdated_ += TimePerFrame;
                currentFrame_ = currentFrame_.WrappedAdd(1, TotalFrames);
            }

            /* We asume that Animations are horizontal strips without any spacing or anything between frames */
            int frameWidth = spriteSheet_.Width / TotalFrames;
            DxVector2 frameOffset;
            DxVector2 drawOffset;
            DxRectangle boundingBox;
            AnimationDescriptor.FrameOffsets.OffsetForFrame(currentFrame_, out frameOffset, out drawOffset,
                out boundingBox);

            Rectangle frameOutline = new Rectangle(frameWidth * currentFrame_, 0, (int) boundingBox.Width,
                (int) boundingBox.Height);
            if(frameOutline.Width == 0)
            {
                frameOutline.Width = frameWidth;
            }
            if(frameOutline.Height == 0)
            {
                frameOutline.Height = spriteSheet_.Height;
            }

            frameOutline.X += (int) frameOffset.X;
            frameOutline.Y += (int) frameOffset.Y;

            position.X += drawOffset.X;
            position.Y += drawOffset.Y;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if(orientation != AnimationDescriptor.Orientation)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            spriteBatch.Draw(spriteSheet_, null,
                new Rectangle((int) position.X, (int) position.Y, (int) (frameOutline.Width * AnimationDescriptor.Scale),
                    (int) (frameOutline.Height * AnimationDescriptor.Scale)), frameOutline, null, 0, null, Color.White,
                spriteEffects, 0);
        }

        public bool LoadContent(ContentManager contentManager)
        {
            spriteSheet_ = contentManager.Load<Texture2D>(AnimationDescriptor.Asset);
            return true;
        }

        /* TODO: Make proper builders for everything */

        public void Reset()
        {
            currentFrame_ = 0;
        }

        protected virtual void DeSerialize()
        {
            LoadContent(DxGame.Instance.Content);
        }

        [OnDeserialized]
        private void BaseDeSerialize(StreamingContext context)
        {
            DeSerialize();
        }
    }
}