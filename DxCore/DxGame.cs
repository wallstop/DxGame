﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DxCore.Core;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Messaging.Game;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Settings;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using Model = DxCore.Core.Models.Model;

namespace DxCore
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

    public abstract class DxGame : Game
    {
        private static readonly object GameLock = new object();

        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected UniqueId GameId { get; }

        protected MessageHandler MessageHandler { get; }

        private static DxGame singleton_;

        protected static T ApplyToSingleton<T>(Func<DxGame, T> singletonFunction) 
        {
            lock(GameLock)
            {
                return singletonFunction.Invoke(singleton_);
            }
        }

        protected static void ApplyToSingleton<T>(Action<DxGame> singletonFunction)
        {
            lock(GameLock)
            {
                singletonFunction.Invoke(singleton_);
            }
        }

        public Scale Scale { get; private set; } = Scale.Medium;

        public Rectangle Screen { get; protected set; }
        public SpriteBatch SpriteBatch { get; protected set; }
        public GraphicsDeviceManager Graphics { get; set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public GameElementCollection DxGameElements { get; protected set; }
        // TODO: Thread safety? Move this to some kind of Context static class?
        public static DxGame Instance => singleton_;
        public double TargetFps => 60.0;
        protected static readonly TimeSpan MinimumFramerate = TimeSpan.FromSeconds(1 / 10000.0);
        public GameElementCollection NewGameElements { get; } = new GameElementCollection();
        public GameElementCollection RemovedGameElements { get; } = new GameElementCollection();

        public GameId GameGuid { get; protected set; }

        public DxGameTime CurrentTime { get; protected set; } = new DxGameTime();

        public UpdateMode UpdateMode { get; set; } = UpdateMode.Active;

        /* TODO: Remove public access to this, this was made public for network testing */
        public List<Model> Models { get; } = new List<Model>();

        protected TimeSpan lastFrameTick_;
        protected TimeSpan compensatedGameTime_;

        protected double timeSkewMilliseconds_;

        public Stopwatch GameTimer { get; }

        // TODO: Move all this crap out somewhow

        public DxRectangle ScreenRegion
        {
            get
            {
                CameraModel cameraModel = Model<CameraModel>();
                if(ReferenceEquals(cameraModel, null))
                {
                    return new DxRectangle(Screen);
                }

                float x = Screen.Width / 2.0f - cameraModel.Position.X;
                float y = Screen.Height / 2.0f - cameraModel.Position.Y;

                // Attempt to bind by map rules
                // TODO: Shove this shit in the camera mdel
                MapModel mapModel = Model<MapModel>();
                if(!ReferenceEquals(mapModel, null))
                {
                    x = MathHelper.Clamp(x,
                        Math.Max(float.MinValue, -(mapModel.MapBounds.X + mapModel.MapBounds.Width - Screen.Width)),
                        mapModel.MapBounds.X);

                    y = MathHelper.Clamp(y,
                        Math.Max(float.MinValue, -(mapModel.MapBounds.Y + mapModel.MapBounds.Height - Screen.Height)),
                        mapModel.MapBounds.Y);
                }

                return new DxRectangle(x, y, Screen.Width, Screen.Height);
            }
        }

        public GameSettings GameSettings { get; }
        public Controls Controls { get; }

        protected abstract void SetUp();

        protected DxGame()
        {
            lock(GameLock)
            {
                Validate.Hard.IsNull(singleton_,
                    "There can only be one instance of the game running in a single process");
                singleton_ = this;
            }

            // TODO: See what parts of this can be offloaded to initialize
            GameSettings = new GameSettings();
            GameSettings.Load();

            Controls = new Controls();
            Controls.Load();

            Screen = new Rectangle(0, 0, GameSettings.ScreenWidth, GameSettings.ScreenHeight);
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Screen.Height,
                PreferredBackBufferWidth = Screen.Width
            };

            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / TargetFps);
            IsFixedTimeStep = false;

            // LOL VSYNC
            Graphics.SynchronizeWithVerticalRetrace = false;

            DxGameElements = new GameElementCollection();
            Content.RootDirectory = "Content";

            GameId = new UniqueId();
            GameGuid = new GameId();

            MessageHandler = new MessageHandler(GameId);
            MessageHandler.RegisterMessageHandler<EntityCreatedMessage>(HandleEntityCreatedMessage);
            MessageHandler.RegisterMessageHandler<EntityRemovedMessage>(HandleEntityRemovedMessage);
            MessageHandler.RegisterMessageHandler<TimeSkewRequest>(HandleTimeSkewRequest);
            timeSkewMilliseconds_ = 0;
            lastFrameTick_ = TimeSpan.Zero;
            compensatedGameTime_ = TimeSpan.Zero;
            GameTimer = Stopwatch.StartNew();
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

        public void ProcessUntypedMessage(Message message)
        {
            dynamic typedMessage = message;
            ProcessTypedMessage(typedMessage);
        }

        public void ProcessTypedMessage<T>(T message) where T : Message
        {
            GlobalMessageBus.TypedBroadcast(message);
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
            }
            else
            {
                Logger.Error($"{nameof(AttachModel)} failed. Model {model} already exists in {Models}");
            }

            return !alreadyExists;
        }

        // TODO: Figure out a better way to attach shit to the game
        protected void AddAndInitializeComponent(Component component)
        {
            component.LoadContent();
            component.Initialize();
            NewGameElements.Add(component);
        }

        private void HandleEntityCreatedMessage(EntityCreatedMessage entityCreated)
        {
            Component createdComponent;
            if(entityCreated.TryGetCreatedEntity(out createdComponent))
            {
                AddAndInitializeComponent(createdComponent);
                return;
            }

            GameObject createdGameObject;
            if(entityCreated.TryGetCreatedEntity(out createdGameObject))
            {
                // TODO: Subtle bug. What about components that are attached to the game state already? IGNORE IGNORE TOO ANNOYING RIGHT NOW
                AddAndInitializeGameObject(createdGameObject);
            }
        }

        protected void HandleEntityRemovedMessage(EntityRemovedMessage entityRemoved)
        {
            Component removedComponent;
            if(entityRemoved.TryGetRemovedEntity(out removedComponent))
            {
                RemoveComponent(removedComponent);
                return;
            }

            GameObject removedGameObject;
            if(entityRemoved.TryGetRemovedEntity(out removedGameObject))
            {
                RemoveGameObject(removedGameObject);
            }
        }

        protected void HandleTimeSkewRequest(TimeSkewRequest timeSkewRequest)
        {
            timeSkewMilliseconds_ = timeSkewRequest.OffsetMilliseconds;
        }

        protected void AddAndInitializeGameObject(GameObject gameObject)
        {
            foreach(var component in gameObject.Components)
            {
                AddAndInitializeComponent(component);
            }
            NewGameElements.Add(gameObject);
        }

        protected void RemoveGameObject(GameObject gameObject)
        {
            if(ReferenceEquals(gameObject, null))
            {
                Logger.Warn($"{nameof(RemoveGameObject)} called with null {typeof(GameObject)}");
                return;
            }
            RemovedGameElements.Add(gameObject);
        }

        protected void RemoveComponent(Component component)
        {
            RemovedGameElements.Add(component);
        }

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            GameObject.From(SpriteBatchEnder.Instance).Create();
            
            new InputModel().Create();
            new CameraModel().Create();
            new CollisionModel().Create();

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

        protected void UpdateElements()
        {
            foreach(object newGameElement in NewGameElements)
            {
                DxGameElements.Add(newGameElement);
            }
            NewGameElements.Clear();
            foreach(object removedGameElement in RemovedGameElements)
            {
                DxGameElements.Remove(removedGameElement);

                Component maybeComponent = removedGameElement as Component;
                if(!ReferenceEquals(maybeComponent, null))
                {
                    maybeComponent.Parent?.RemoveComponents(maybeComponent);
                    maybeComponent.Parent = null;
                    continue;
                }

                GameObject maybeGameObject = removedGameElement as GameObject;
                if(!ReferenceEquals(maybeGameObject, null))
                {
                    foreach(Component component in maybeGameObject.Components)
                    {
                        RemoveComponent(component);
                    }
                }
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
            DxGameTime dxGameTime = DetermineGameTime(gameTime);
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
                    CooperativeUpdate(dxGameTime);
                    break;
                case UpdateMode.Passive:
                    PassiveUpdate(dxGameTime);
                    break;
                default:
                    throw new NotImplementedException($"{UpdateMode} currently not implemented");
            }
        }

        protected DxGameTime DetermineGameTime(GameTime gameTime)
        {
            TimeSpan wallclock;
            TimeSpan actualElapsed;
            while((actualElapsed = (wallclock = GameTimer.Elapsed) - lastFrameTick_) < MinimumFramerate)
            {
                // Chill out dawg, let's hang out here
            }

            TimeSpan elapsed = MathUtils.Min(actualElapsed, TargetElapsedTime);
            TimeSpan currentTime = compensatedGameTime_ + elapsed;
            if(TargetElapsedTime.TotalMilliseconds < Math.Abs(timeSkewMilliseconds_))
            {
                if(0 < timeSkewMilliseconds_)
                {
                    TimeSpan timeSkew = TimeSpan.FromMilliseconds(timeSkewMilliseconds_);
                    currentTime += timeSkew;
                    compensatedGameTime_ += timeSkew;
                }
                else
                {
                    TimeSpan timeSkew = TimeSpan.FromMilliseconds(-timeSkewMilliseconds_);
                    currentTime -= timeSkew;
                    compensatedGameTime_ -= timeSkew;
                }
                // TODO: Roll this much more gently
                timeSkewMilliseconds_ = 0;
            }
            bool isRunningSlowly = TargetElapsedTime < actualElapsed;
            DxGameTime dxGameTime = new DxGameTime(currentTime, elapsed, isRunningSlowly);
            lastFrameTick_ = wallclock;
            compensatedGameTime_ += elapsed;
            return dxGameTime;
        }

        protected void PassiveUpdate(DxGameTime gameTime)
        {
            NetworkModel networkModel = Model<NetworkModel>();
            networkModel?.ReceiveData(gameTime);
            networkModel?.SendData(gameTime);
        }

        protected void CooperativeUpdate(DxGameTime gameTime)
        {
            NetworkModel networkModel = Model<NetworkModel>();
            InputModel inputModel = Model<InputModel>();
            inputModel?.Process(gameTime);

            networkModel?.ReceiveData(gameTime);
            networkModel?.Process(gameTime);

            // TODO: Move this out of here
            DeveloperModel developerModel = Model<DeveloperModel>();
            developerModel.Process(gameTime);

            UpdateElements();
            networkModel?.SendData(gameTime);
        }

        protected void ActiveUpdate(DxGameTime gameTime)
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
            networkModel?.ReceiveData(gameTime);
            /* We may end up modifying these as we iterate over them, so take an immutable copy */
            foreach(var processable in DxGameElements.Processables)
            {
                processable.Process(gameTime);
            }
            networkModel?.SendData(gameTime);
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

        protected override void OnExiting(object sender, EventArgs args)
        {
            lock(GameLock)
            {
                Validate.Hard.IsNotNull(singleton_, () => "Cannot exit a null game!");
                singleton_ = null;
            }
            base.OnExiting(sender, args);
        }
    }
}