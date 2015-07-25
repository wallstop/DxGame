using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Basic;
using DXGame.Core.Menus;
using DXGame.Core.Models;
using DXGame.Core.Settings;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Model = DXGame.Core.Models.Model;

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

        private static readonly Lazy<DxGame> singleton_ =
            new Lazy<DxGame>(() => new DxGame());

        private readonly List<Model> models_ = new List<Model>();
        public Rectangle Screen { get; protected set; }
        public SpriteBatch SpriteBatch { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public ComponentCollection DxComponents { get; private set; }
        // TODO: Thread safety? Move this to some kind of Context static class?
        public static DxGame Instance => singleton_.Value;

        public DxRectangle ScreenRegion
        {
            get
            {
                GameModel gameModel = Model<GameModel>();
                if (Check.IsNullOrDefault(gameModel))
                {
                    return new DxRectangle(Screen);
                }
                MapModel mapModel = Model<MapModel>();
                float x = Screen.Width / 2.0f - gameModel.FocalPoint.Position.X;
                x = MathHelper.Clamp(x,
                    Math.Max(float.MinValue,
                        -(mapModel.MapBounds.X + mapModel.MapBounds.Width - Screen.Width)),
                    mapModel.MapBounds.X);

                float y = Screen.Height / 2.0f - gameModel.FocalPoint.Position.Y;
                y = MathHelper.Clamp(y,
                    Math.Max(0, mapModel.MapBounds.Y + mapModel.MapBounds.Height - Screen.Height),
                    mapModel.MapBounds.Y);

                return new DxRectangle(x, y, Screen.Width, Screen.Height);
            }
        }

        public GameSettings GameSettings { get; set; }

        private DxGame()
        {
            // TODO: See what parts of this can be offloaded to initialize
            GameSettings = new GameSettings();
            GameSettings.Load();

            Screen = new Rectangle(0, 0, GameSettings.ScreenWidth, GameSettings.ScreenHeight);
            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Screen.Height,
                PreferredBackBufferWidth = Screen.Width
            };

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);
            IsFixedTimeStep = false;

            // LOL VSYNC
            graphics.SynchronizeWithVerticalRetrace = false;

            DxComponents = new ComponentCollection();
            Content.RootDirectory = "Content";
        }

        public T Model<T>() where T : Model
        {
            return models_.OfType<T>().FirstOrDefault();
        }

        public bool AttachModel(Model model)
        {
            bool alreadyExists = models_.Contains(model);
            if (!alreadyExists)
            {
                models_.Add(model);
                AddAndInitializeComponent(model);
            }
            else
            {
                LOG.Error($"{nameof(AttachModel)} failed. Model {model} already exists in {models_}");
            }

            return !alreadyExists;
        }

        public void AddAndInitializeComponent(Component component)
        {
            component.Initialize();
            DxComponents.Add(component);
        }

        public void AddAndInitializeComponents(params Component[] components)
        {
            foreach (var component in components)
            {
                AddAndInitializeComponent(component);
            }
        }

        public void AddAndInitializeGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (
                var component in
                    gameObjects.Select(gameObject => gameObject.Components)
                        .SelectMany(components => components))
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
            foreach (var component in gameObject.Components)
            {
                DxComponents.Remove(component);
            }
        }

        public void RemoveComponent(Component component)
        {
            DxComponents.Remove(component);
        }

        public void RemoveComponents(params Component[] components)
        {
            foreach (var component in components)
            {
                RemoveComponent(component);
            }
        }

        // TODO: Remove. This is currently only for testing
        public void ResetComponents(IEnumerable<Component> components)
        {
            Components.Clear();
            foreach (var gameComponent in components)
            {
                AddAndInitializeComponent(gameComponent);
            }
        }

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            AddAndInitializeComponent(new SpriteBatchInitializer(this));
            AddAndInitializeComponent(new SpriteBatchEnder(this));
            var playMenu = new MainMenu(this);
            AddAndInitializeComponent(playMenu);

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
            var dxGameTime = new DxGameTime(gameTime);

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
            networkModel.ReceiveData(dxGameTime);
            List<Component> currentComponents = new List<Component>(DxComponents.Components());
            foreach (Component component in currentComponents)
            {
                component.Process(dxGameTime);
            }
            networkModel.SendData(dxGameTime);
            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var dxGameTime = new DxGameTime(gameTime);
            foreach (DrawableComponent drawable in DxComponents.Drawables())
            {
                drawable.Draw(dxGameTime);
            }
            base.Draw(gameTime);
        }
    }
}