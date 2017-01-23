using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Extension;
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

        public TimeSpan TimePerFrame
            => TimeSpan.Zero.FromAccurateMilliseconds(1000.0 / AnimationDescriptor.FramesPerSecond);

        [DataMember]
        private int CurrentFrame { get; set; }

        [IgnoreDataMember]
        private Func<ContentManager, Texture2D> CustomLoadContent { get; set; }

        [DataMember]
        private TimeSpan LastUpdated { get; set; }

        private int TotalFrames => AnimationDescriptor.FrameCount;

        public Animation(AnimationDescriptor descriptor, DrawPriority drawPriority = DrawPriority.Normal,
            Func<ContentManager, Texture2D> customLoadContent = null)
        {
            Validate.Hard.IsNotNullOrDefault(descriptor, () => this.GetFormattedNullOrDefaultMessage(descriptor));
            Validate.Hard.IsPositive(descriptor.FrameCount,
                () => $"Cannot initialize an {nameof(Animation)} with a FrameCount of {descriptor.FrameCount}");
            Validate.Hard.IsPositive(descriptor.FramesPerSecond,
                () =>
                    $"Cannot initialize an {nameof(Animation)} with a {nameof(descriptor.FramesPerSecond)} of {descriptor.FramesPerSecond}");
            Validate.Hard.IsNotNullOrDefault(descriptor.Asset,
                () => this.GetFormattedNullOrDefaultMessage(nameof(descriptor.Asset)));
            CustomLoadContent = customLoadContent;
            AnimationDescriptor = descriptor;
            DrawPriority = drawPriority;
            LastUpdated = DxGame.Instance.CurrentDrawTime.TotalGameTime;
        }

        public void Draw(SpriteBatch spriteBatch, DxGameTime gameTime, DxVector2 position, Direction orientation)
        {
            if(Validate.Check.IsNull(AnimationDescriptor.Asset))
            {
                return;
            }

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

        public void DrawDebug(SpriteBatch spriteBatch, DxGameTime gameTime, DxVector2 position, Direction orientation)
        {
            Draw(spriteBatch, gameTime, position, orientation);

            spriteBatch.DrawString(FontFactory.Instance.Default, (CurrentFrame + 1).ToString(), position.Vector2,
                ColorFactory.Transparency(0.5f, Color.Purple));
        }

        public bool LoadContent(ContentManager contentManager)
        {
            Texture2D spriteSheet;
            if(!Validate.Check.IsNull(CustomLoadContent))
            {
                spriteSheet = CustomLoadContent(contentManager);
            }
            else
            {
                spriteSheet = DefaultLoadContent(contentManager);
            }
            spriteSheet_ = spriteSheet;
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

        private Texture2D DefaultLoadContent(ContentManager contentManager)
        {
            return contentManager.Load<Texture2D>(AnimationDescriptor.Asset);
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