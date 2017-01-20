using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Basic
{
    /**
    <summary>
        DrawableComponent forms the base of all Components that wish to be rendered at some point in time.
        Each derived class must implement their own LoadContent and Draw methods, which will be called
        in the main LoadContent and Draw methods. 

        Components using this class will typically be sprites, with or without animations. Particle effects, etc.
            
        <see cref=SimpleSpriteComponent />

        For other base components, 
        <see cref=InitializbleComponent />
        <see cref=UpdateableComponent />
    </summary>
    */

    [Serializable]
    [DataContract]
    public abstract class DrawableComponent : Component, IComparable<DrawableComponent>, IDrawable
    {
        protected DrawableComponent(DrawPriority drawPriority = DrawPriority.Normal)
        {
            DrawPriority = drawPriority;
        }

        public int CompareTo(DrawableComponent other)
        {
            return DrawPriority.CompareTo(other?.DrawPriority);
        }

        [DataMember]
        public DrawPriority DrawPriority { get; protected set; }

        public abstract void Draw(SpriteBatch spriteBatch, DxGameTime gameTime);

        public int CompareTo(IDrawable other)
        {
            return Drawable.DefaultComparer.Compare(this, other);
        }
    }
}