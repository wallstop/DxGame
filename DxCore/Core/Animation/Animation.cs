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
        [NonSerialized] [IgnoreDataMember] private Texture2D spriteSheet_;

        [DataMember]
        public AnimationDescriptor AnimationDescriptor { get; private set; }

        [DataMember]
        public DrawPriority DrawPriority { get; private set; }

        public TimeSpan TimePerFrame => TimeSpan.FromSeconds(1.0f / AnimationDescriptor.FramesPerSecond);

        [DataMember]
        private int CurrentFrame { get; set; }

        [DataMember]
        private TimeSpan LastUpdated { get; set; }

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
            DrawPriority = drawPriority;
            LastUpdated = DxGame.Instance.CurrentDrawTime.TotalGameTime;
        }

        public void Draw(SpriteBatch spriteBatch, DxGameTime gameTime, DxVector2 position, Direction orientation)
        {
            UpdateToCurrentFrame(gameTime);

            DxVector2 frameOffset;
            DxVector2 drawOffset;
            int frameWidth;
            int frameHeight;
            AnimationDescriptor.OffsetForFrame(CurrentFrame, out frameOffset, out drawOffset, out frameWidth,
                out frameHeight);

            Rectangle frameOutline = new Rectangle((int) Math.Round(frameOffset.X), (int) Math.Round(frameOffset.Y),
                frameWidth, frameHeight);

            position.X += drawOffset.X;
            position.Y += drawOffset.Y;

            Rectangle drawLocation = new DxRectangle(position, frameWidth * AnimationDescriptor.Scale,
                frameHeight * AnimationDescriptor.Scale);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if(orientation != AnimationDescriptor.Orientation)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            spriteBatch.Draw(spriteSheet_, null, drawLocation, frameOutline, null, 0, null, Color.White, spriteEffects,
                0);
        }

        public bool LoadContent(ContentManager contentManager)
        {
            spriteSheet_ = contentManager.Load<Texture2D>(AnimationDescriptor.Asset);
            return true;
        }

        /* TODO: Make proper builders for everything */

        public void Reset()
        {
            CurrentFrame = 0;
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

        private void UpdateToCurrentFrame(DxGameTime gameTime)
        {
            while(LastUpdated + TimePerFrame < gameTime.TotalGameTime)
            {
                LastUpdated += TimePerFrame;
                CurrentFrame = CurrentFrame.WrappedAdd(1, TotalFrames);
            }
        }
    }
}