#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Menus;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using log4net;
using Microsoft.Xna.Framework;
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

        public DxGame()
        {
            var playMenu = new MainMenu(this);
        }

        public T Model<T>() where T : GameComponent
        {
            return models_.OfType<T>().First();
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
                MapModel mapModel = Model<MapModel>();
                float x = gameModel.Screen.Width / 2.0f - gameModel.FocalPoint.Position.X;
                x = MathUtils.Constrain(x,
                    Math.Max(float.MinValue, -(mapModel.MapBounds.X + mapModel.MapBounds.Width - gameModel.Screen.Width)),
                    mapModel.MapBounds.X);

                float y = gameModel.Screen.Height / 2.0f - gameModel.FocalPoint.Position.Y;
                y = MathUtils.Constrain(y,
                    Math.Max(0, mapModel.MapBounds.Y + mapModel.MapBounds.Height - gameModel.Screen.Height),
                    mapModel.MapBounds.Y);

                return new Rectangle2f(x, y, gameModel.Screen.Width, gameModel.Screen.Height);
            }

        }

        protected override void Initialize()
        {
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

            base.Update(gameTime);
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