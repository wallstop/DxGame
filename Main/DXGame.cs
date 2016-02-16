﻿using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Basic;
using DXGame.Core.Menus;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Settings;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using Model = DXGame.Core.Models.Model;

namespace DXGame.Main
{
    /*
        The way in which the game updates. If we are a server, or singleplayer, we will be in an "Active" state. That is, once we read input from our connected clients, 
        WE decide exactly what the state should be, and dictate it to everyone else.
        For connected clients, we need to be in "Cooperative" mode. That is, we need to do client-side prediction, update our state accordingly, but also take in over-rules
        from the server in case an action / soemthing that we thought happened didn't actually happen. In that way, we are "cooperating" with the server.
        In Passive mode, no action that we take has any effect. Our state is treated as "read only" from our point of view. This is useful for things like "spectating" or
        simple network tests.
    */

    public enum UpdateMode
    {
        Active,
        Cooperative,
        Passive
    }

    public class DxGame : Game
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<DxGame> singleton_ = new Lazy<DxGame>(() => new DxGame());

        public Rectangle Screen { get; protected set; }
        public SpriteBatch SpriteBatch { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public GameElementCollection DxGameElements { get; private set; }
        // TODO: Thread safety? Move this to some kind of Context static class?
        public static DxGame Instance => singleton_.Value;
        public double TargetFps => 60.0;
        public GameElementCollection NewGameElements { get; } = new GameElementCollection();
        public GameElementCollection RemovedGameElements { get; } = new GameElementCollection();

        public DxGameTime CurrentTime { get; private set; } = new DxGameTime();

        public UpdateMode UpdateMode { get; set; } = UpdateMode.Active;

        /* TODO: Remove public access to this, this was made public for network testing */
        public List<Model> Models { get; } = new List<Model>();

        public DxRectangle ScreenRegion
        {
            get
            {
                GameModel gameModel = Model<GameModel>();
                if(Check.IsNullOrDefault(gameModel))
                {
                    return new DxRectangle(Screen);
                }
                var mapModel = Model<MapModel>();
                float x = Screen.Width / 2.0f - gameModel.FocalPoint.Position.X;
                x = MathHelper.Clamp(x,
                    Math.Max(float.MinValue, -(mapModel.MapBounds.X + mapModel.MapBounds.Width - Screen.Width)),
                    mapModel.MapBounds.X);

                float y = Screen.Height / 2.0f - gameModel.FocalPoint.Position.Y;
                y = MathHelper.Clamp(y,
                    Math.Max(float.MinValue, -(mapModel.MapBounds.Y + mapModel.MapBounds.Height - Screen.Height)),
                    mapModel.MapBounds.Y);

                return new DxRectangle(x, y, Screen.Width, Screen.Height);
            }
        }

        public GameSettings GameSettings { get; }
        public Controls Controls { get; }

        private DxGame()
        {
            // TODO: See what parts of this can be offloaded to initialize
            GameSettings = new GameSettings();
            GameSettings.Load();

            Controls = new Controls();
            Controls.Load();

            Screen = new Rectangle(0, 0, GameSettings.ScreenWidth, GameSettings.ScreenHeight);
            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Screen.Height,
                PreferredBackBufferWidth = Screen.Width
            };

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / TargetFps);
            IsFixedTimeStep = false;

            // LOL VSYNC
            graphics.SynchronizeWithVerticalRetrace = false;

            DxGameElements = new GameElementCollection();
            Content.RootDirectory = "Content";
        }

        /**
            <summary>
                Given some DxVector2 that represents an offset from the screen, not an actual point in space, 
                returns the coordinates that represent that offset from the Screen in "real world" (map) coordinates.
                
                This is incredibly useful for drawing HUD-type widgets.
            </summary>
        */

        public Vector2 OffsetFromScreen(DxVector2 offset)
        {
            DxRectangle screenRegion = ScreenRegion;
            Vector2 drawLocation = new Vector2(Math.Abs(screenRegion.X) + offset.X, Math.Abs(screenRegion.Y) + offset.Y);
            return drawLocation;
        }

        /**

            <summary>
                Broadcasts a message to all Models
            </summary>
        */

        public void BroadcastUntypedMessage(Message message)
        {
            foreach(var model in Models)
            {
                model.MessageHandler.HandleUntypedMessage(message);
            }
        }

        public void BroadcastTypedMessage<T>(T message) where T : Message
        {
            foreach(var model in Models)
            {
                model.MessageHandler.HandleTypedMessage<T>(message);
            }
        }

        public T Model<T>() where T : Model
        {
            return Models.OfType<T>().FirstOrDefault();
        }

        public bool AttachModel(Model model)
        {
            bool alreadyExists = Models.Contains(model);
            if(!alreadyExists)
            {
                Models.Add(model);
                AddAndInitializeComponent(model);
            }
            else
            {
                LOG.Error($"{nameof(AttachModel)} failed. Model {model} already exists in {Models}");
            }

            return !alreadyExists;
        }

        // TODO: Figure out a better way to attach shit to the game
        public void AddAndInitializeComponent(Component component)
        {
            component.LoadContent();
            component.Initialize();
            NewGameElements.Add(component);
        }

        public void AddAndInitializeComponents(params Component[] components)
        {
            foreach(var component in components)
            {
                AddAndInitializeComponent(component);
            }
        }

        public void AddAndInitializeComponents(IEnumerable<Component> components)
        {
            foreach(var component in components)
            {
                AddAndInitializeComponent(component);
            }
        }

        public void AddAndInitializeGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach(var gameObject in gameObjects)
            {
                AddAndInitializeGameObject(gameObject);
            }
        }

        public void AddAndInitializeGameObject(GameObject gameObject)
        {
            foreach(var component in gameObject.Components)
            {
                AddAndInitializeComponent(component);
            }
            NewGameElements.Add(gameObject);
        }

        public void AddGameObject(GameObject gameObject)
        {
            Validate.IsNotNull(gameObject);
            NewGameElements.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if(ReferenceEquals(gameObject, null))
            {
                LOG.Warn($"{nameof(RemoveGameObject)} called with null {typeof(GameObject)}");
                return;
            }

            gameObject.Dispose();
            foreach(var component in gameObject.Components)
            {
                RemovedGameElements.Add(component);
            }
            RemovedGameElements.Add(gameObject);
        }

        public void RemoveComponent(Component component)
        {
            RemovedGameElements.Add(component);
        }

        public void Remove(object element)
        {
            RemovedGameElements.Add(element);
        }

        // TODO: Figure out a better way to remove shit from the game
        public void RemoveComponents(params Component[] components)
        {
            foreach(var component in components)
            {
                RemoveComponent(component);
            }
        }

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            AddAndInitializeComponent(new SpriteBatchInitializer());
            AddAndInitializeComponent(new SpriteBatchEnder());
            var playMenu = new MainMenu();
            AddAndInitializeComponent(playMenu);

            var frameModel = new FrameModel();
            AttachModel(frameModel);

            var netModel = new NetworkModel();
            AttachModel(netModel);

            var inputModel = new InputModel();
            AttachModel(inputModel);

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

        private void UpdateElements()
        {
            foreach(object newGameElement in NewGameElements)
            {
                DxGameElements.Add(newGameElement);
                EntityStatusChangedMessage entityStatusChanged = new EntityStatusChangedMessage
                {
                    Entity = newGameElement,
                    Status = EntityStatus.Created
                };
                BroadcastTypedMessage<EntityStatusChangedMessage>(entityStatusChanged);
            }
            NewGameElements.Clear();
            foreach(object removedGameElement in RemovedGameElements)
            {
                DxGameElements.Remove(removedGameElement);
                EntityStatusChangedMessage entityStatusChanged = new EntityStatusChangedMessage
                {
                    Entity = removedGameElement,
                    Status = EntityStatus.Removed
                };
                BroadcastTypedMessage<EntityStatusChangedMessage>(entityStatusChanged);
            }
            RemovedGameElements.Clear();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var dxGameTime = new DxGameTime(gameTime);
            CurrentTime = dxGameTime;

            // Querying Gamepad.GetState(...) requires xinput1_3.dll (The xbox 360 controller driver). Interesting fact...
            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch(UpdateMode)
            {
                case UpdateMode.Active:
                    ActiveUpdate(dxGameTime);
                    break;
                case UpdateMode.Cooperative:
                    throw new NotImplementedException($"{UpdateMode} currently not implemented");
                case UpdateMode.Passive:
                    PassiveUpdate(dxGameTime);
                    break;
                default:
                    throw new NotImplementedException($"{UpdateMode} currently not implemented");
            }
        }

        private void PassiveUpdate(DxGameTime gameTime)
        {
            var networkModel = Model<NetworkModel>();
            networkModel.ReceiveData(gameTime);
            networkModel.SendData(gameTime);
        }

        private void ActiveUpdate(DxGameTime gameTime)
        {
            // TODO: Fix update & draw lockstep so they're de-synced. Sync Update() to 60/30/whatever FPS, Draw() unlocked (or locked to player preference)

            // TODO (Short term): Have a dedicated InputSystem 
            /*
                TODO (Long term): XNA/Monogame only supports polling-based input. In order to get this to the level of granularity that
                I'd like, we need to have a dedicated InputSystem that is capable of short-polling (on the order of 1/10th of a millisecond) the keyboard/gamepad/mouse state, 
                diffing the previous state, and publishing events if there is a change. Then it's a matter of hooking up subscribers to these events.
            */
            var networkModel = Model<NetworkModel>();

            // Should probably thread this... but wait until we have perf testing :)
            networkModel.ReceiveData(gameTime);
            /* We may end up modifying these as we iterate over them, so take an immutable copy */
            foreach(var processable in DxGameElements.Processables)
            {
                processable.Process(gameTime);
            }
            networkModel.SendData(gameTime);
            UpdateElements();

            base.Update(gameTime.ToGameTime());
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var dxGameTime = new DxGameTime(gameTime);
            /* We may end up modifying these as we iterate over them, so take an immutable copy */
            foreach(var drawable in DxGameElements.Drawables)
            {
                drawable.Draw(SpriteBatch, dxGameTime);
            }
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            var networkModel = Model<NetworkModel>();
            networkModel?.ShutDown();
            base.Dispose(disposing);
        }
    }
}