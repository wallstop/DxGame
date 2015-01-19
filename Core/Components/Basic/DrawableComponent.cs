using System.Diagnostics;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Basic
{
    public enum DrawPriority
    {
        INIT_SPRITEBATCH = -100,
        HIGHEST = -99,
        HIGH = 1,
        NORMAL = 5,
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

    public class DrawableComponent : DrawableGameComponent
    {
        /**
            Note: This id_ field is the UniqueId of the Component, *NOT* of the GameObject. 
            This is a very important distinction.
        */
        protected readonly UniqueId id_ = new UniqueId();

        public GameObject Parent { get; set; }

        protected SpriteBatch spriteBatch_;

        public UniqueId Id
        {
            get { return id_; }
        }

        public DxGame DxGame
        {
            get { return (DxGame) Game; }
        }

        public DrawableComponent(DxGame game)
            : base(game)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(game), "DrawableComponent cannot be initialized with a null game");
            spriteBatch_ = game.SpriteBatch;
            UpdatePriority = UpdatePriority.NORMAL;
            DrawPriority = DrawPriority.NORMAL;
        }

        protected UpdatePriority UpdatePriority
        {
            set { UpdateOrder = (int) value; }
            get { return (UpdatePriority) UpdateOrder; }
        }

        protected DrawPriority DrawPriority
        {
            set { DrawOrder = (int) value; }
            get { return (DrawPriority) DrawOrder; }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public virtual void HandleMessage(Message message)
        {
        }
    }
}