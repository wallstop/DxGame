#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Basic;
using DXGame.Core.Menus;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace DXGame.Main
{
    public enum InteractionState
    {
        None,
        StartMenu,
        Loading,
        Playing,
        Paused
    }

    public enum GameRole
    {
        None,
        DedicatedServer,
        HostedServer,
        Client
    }

    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class DxGame : Game
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (DxGame));
        private readonly List<GameComponent> models_ = new List<GameComponent>();

        public Rectangle Screen { get; protected set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public DxGame()
        {
            Screen = new Rectangle(0, 0, 1280, 720);
            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Screen.Height,
                PreferredBackBufferWidth = Screen.Width
            };

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
        }

        public T Model<T>() where T : GameComponent
        {
            return models_.OfType<T>().FirstOrDefault();
        }

        public bool AttachModel(GameComponent model)
        {
            bool alreadyExists = models_.Contains(model);
            if (!alreadyExists)
            {
                models_.Add(model);
            }
            else
            {
                LOG.Error(String.Format("AttachModel failed. Model {0} already exists in {1}", model, models_));
            }

            return !alreadyExists;
        }

        public Rectangle2f ScreenRegion
        {
            get
            {
                GameModel gameModel = Model<GameModel>();
                if(GenericUtils.IsNullOrDefault(gameModel))
                {
                    return new Rectangle2f(Screen);
                }
                MapModel mapModel = Model<MapModel>();
                float x = Screen.Width / 2.0f - gameModel.FocalPoint.Position.X;
                x = MathUtils.Constrain(x,
                    Math.Max(float.MinValue, -(mapModel.MapBounds.X + mapModel.MapBounds.Width - Screen.Width)),
                    mapModel.MapBounds.X);

                float y = Screen.Height / 2.0f - gameModel.FocalPoint.Position.Y;
                y = MathUtils.Constrain(y,
                    Math.Max(0, mapModel.MapBounds.Y + mapModel.MapBounds.Height - Screen.Height),
                    mapModel.MapBounds.Y);

                return new Rectangle2f(x, y, Screen.Width, Screen.Height);
            }

        }

        public void AddAndInitializeComponent(GameComponent component)
        {
            Components.Add(component);
        }

        public void AddAndInitializeComponents(params GameComponent [] components)
        {
            foreach (GameComponent component in components)
            {
                AddAndInitializeComponent(component);
            }
        }

        public void AddAndInitializeGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var component in gameObjects.Select(gameObject => gameObject.Components).SelectMany(components => components))
            {
                AddAndInitializeComponent(component);
            }
        }

        public void AddAndInitializeGameObject(GameObject gameObject)
        {
            foreach (var component in gameObject.Components)
            {
                AddAndInitializeComponent(component);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            List<GameComponent> components = gameObject.Components;
            foreach (var component in components)
            {
                Components.Remove(component);
            }
        }

        public void RemoveComponent(GameComponent component)
        {
            Components.Remove(component);
        }

        public void RemoveComponents(params GameComponent[] components)
        {
            foreach (var component in components)
            {
                RemoveComponent(component);
            }
        }

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Components.Add(new SpriteBatchInitializer(this));
            Components.Add(new SpriteBatchEnder(this));
            var playMenu = new MainMenu(this);
            Components.Add(playMenu);

            var frameModel = new FrameModel(this);
            AttachModel(frameModel);

            var netModel = new NetworkModel(this);
            AttachModel(netModel);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Querying Gamepad.GetState(...) requires xinput1_3.dll (The xbox 360 controller driver). Interesting fact...
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Fix update & draw lockstep so they're de-synced. Sync Update() to 60/30/whatever FPS, Draw() unlocked (or locked to player preference)

            // TODO (Short term): Have a dedicated InputSystem 
            /*
                TODO (Long term): XNA/Monogame only supports polling-based input. In order to get this to the level of granularity that
                I'd like, we need to have a dedicated InputSystem that is capable of short-polling (on the order of 1/10th of a millisecond) the keyboard/gamepad/mouse state, 
                diffing the previous state, and publishing events if there is a change. Then it's a matter of hooking up subscribers to these events.
            */
            var networkModel = Model<NetworkModel>();

            // Should probably thread this... but wait until we have perf testing :)
            networkModel.ReceiveData();
            base.Update(gameTime);
            networkModel.SendData();
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}