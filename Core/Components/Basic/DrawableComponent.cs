using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Basic
{
    public enum DrawPriority
    {
        INIT_SPRITEBATCH = -100,
        HIGHEST = -99,
        HIGH = 1,
        NORMAL = 5,
        HUD_LAYER = 8,
        LOW = 10,
        END_SPRITEBATCH = 1000
    }

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
    public abstract class DrawableComponent : Component
    {
        [NonSerialized] [IgnoreDataMember] protected SpriteBatch spriteBatch_;

        [DataMember]
        public DrawPriority DrawPriority { get; set; }

        protected DrawableComponent(DxGame game)
            : base(game)
        {
            Validate.IsNotNullOrDefault(game, $"Cannot initialize {GetType()} with a null/default DxGame");
            spriteBatch_ = game.SpriteBatch;
            UpdatePriority = UpdatePriority.NORMAL;
            DrawPriority = DrawPriority.NORMAL;
        }

        public abstract void Draw(DxGameTime gameTime);

        protected override void Update(DxGameTime gameTime)
        {
        }
    }
}