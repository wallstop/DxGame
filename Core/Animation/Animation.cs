using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProtoBuf;

namespace DXGame.Core.Animation
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class Animation : IDrawable, IProcessable
    {
        [ProtoMember(1)] [DataMember] private int currentFrame_;
        [ProtoMember(2)] [DataMember] private DrawPriority drawPriority_;
        [ProtoMember(3)] [DataMember] private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);
        [ProtoMember(4)] [DataMember] protected PositionalComponent position_;
        [NonSerialized] [IgnoreDataMember] private Texture2D spriteSheet_;
        public TimeSpan TimePerFrame => TimeSpan.FromSeconds(1.0f / AnimationDescriptor.FramesPerSecond);

        [DataMember]
        public AnimationDescriptor AnimationDescriptor { get; private set; }

        /* 
            TODO: Precompute the bounding box of each animation frame. Have the animation offer the actual bounding box of the object instead of relying on hardcoded values.
            This can be done by walking the "frame" in each direction and determining the x & y min & max for non-alpha blended pixels 
        */

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

        public void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // Assume gameTime is non-null (if it is we have some issues)
            /* We asume that Animations are horizontal strips without any spacing or anything between frames */
            int frameWidth = spriteSheet_.Width / TotalFrames;
            Rectangle frameOutline = new Rectangle(frameWidth * currentFrame_, 0, frameWidth, spriteSheet_.Height);
            spriteBatch.Draw(spriteSheet_, position_.Position.ToVector2(), frameOutline, Color.White, 0.0f, Vector2.Zero,
                Scale, SpriteEffects.None, 0);
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

        public void Process(DxGameTime gameTime)
        {
            if(gameTime.TotalGameTime >= lastUpdated_ + TimePerFrame)
            {
                lastUpdated_ = gameTime.TotalGameTime;
                currentFrame_ = currentFrame_.WrappedAdd(1, TotalFrames);
            }
        }

        /* TODO: Make proper builders for everything */

        public Animation WithPosition(PositionalComponent position)
        {
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));
            position_ = position;
            return this;
        }

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