using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Animation
{
    [Serializable]
    [DataContract]
    public class Animation
    {
        [DataMember] private int currentFrame_;
        [DataMember] private DrawPriority drawPriority_;
        [DataMember] private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);

        [NonSerialized] [IgnoreDataMember] private Texture2D spriteSheet_;

        public TimeSpan TimePerFrame => TimeSpan.FromSeconds(1.0f / AnimationDescriptor.FramesPerSecond);

        [DataMember]
        public AnimationDescriptor AnimationDescriptor { get; private set; }

        private int TotalFrames => AnimationDescriptor.FrameCount;
        private float Scale => (float) AnimationDescriptor.Scale;

        public Animation(AnimationDescriptor descriptor, DrawPriority drawPriority = DrawPriority.NORMAL)
        {
            Validate.IsNotNullOrDefault(descriptor, StringUtils.GetFormattedNullOrDefaultMessage(this, descriptor));
            Validate.IsTrue(descriptor.FrameCount > 0,
                $"Cannot initialize an {nameof(Animation)} with a FrameCount of {descriptor.FrameCount}");
            Validate.IsNotNullOrDefault(descriptor.FramesPerSecond > 0,
                $"Cannot initialize an {nameof(Animation)} with a {nameof(descriptor.FramesPerSecond)} of {descriptor.FramesPerSecond}");
            Validate.IsNotNullOrDefault(descriptor.Asset,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor.Asset)));
            AnimationDescriptor = descriptor;
            drawPriority_ = drawPriority;
        }

        public DrawPriority DrawPriority => drawPriority_;

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
            AnimationDescriptor.FrameOffsets.OffsetForFrame(currentFrame_, out frameOffset, out drawOffset, out boundingBox);

            Rectangle frameOutline = new Rectangle(frameWidth * currentFrame_, 0, (int) boundingBox.Width, (int) boundingBox.Height);
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

        public int CompareTo(IDrawable other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IProcessable other)
        {
            throw new NotImplementedException();
        }

        public UpdatePriority UpdatePriority => UpdatePriority.NORMAL;

        /* TODO: Make proper builders for everything */

        public void Reset()
        {
            currentFrame_ = 0;
        }

        public bool LoadContent(ContentManager contentManager)
        {
            spriteSheet_ = contentManager.Load<Texture2D>(AnimationDescriptor.Asset);
            return true;
        }

        [OnDeserialized]
        private void BaseDeSerialize(StreamingContext context)
        {
            DeSerialize();
        }

        protected virtual void DeSerialize()
        {
            LoadContent(DxGame.Instance.Content);
        }
    }
}