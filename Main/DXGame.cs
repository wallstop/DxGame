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
            /*
                    TODO: Only draw the objects that are on-screen at the current time. This can be done via naive methods, 
                    such as iterate-over-all-objects and only draw those on screen, or advanced techniques, like http://gamedev.stackexchange.com/questions/14713/culling-for-a-2d-platformer-game,
                    http://www.codeproject.com/Articles/18113/KD-Tree-Searching-in-N-dimensions-Part-I, http://qstuff.blogspot.com/2008/05/spatial-sorting-with-kd-trees-part-1.html.
                */

            /*
                    TODO: Instead of doing a clear, see if we can take a diff. That way, we only have to re-draw certain objects (the ones that have changed)
                */
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Oh god what have I done? Do some proper math, hidden in a function, like a CameraUtils class or something
            float x = width_ / 2.0f - playerSpace_.Center.X;
            x = MathUtils.Constrain(x, Math.Max(float.MinValue, -(mapBounds_.X + mapBounds_.Width - width_)),
                mapBounds_.X);
            float y = height_ / 2.0f - playerSpace_.Center.Y;
            y = MathUtils.Constrain(y, Math.Max(0, (mapBounds_.Y + mapBounds_.Height - height_)), mapBounds_.Y);

            Matrix cameraShift = Matrix.CreateTranslation(x, y, 0);
            spriteBatch_.Begin(0, null, null, null, null, null, cameraShift);

            var screenRegion = new Rectangle(0 - (int) x, 0 - (int) y, width_, height_);
            var map = GameModel.Model<MapModel>();
            var mapObjects = map.ObjectsInRange(screenRegion);
            var drawables = GameObjectUtils.ComponentsOfType<DrawableComponent>(mapObjects);

            // Draw map items
            foreach (DrawableComponent component in drawables)
            {
                component.Draw(spriteBatch_);
            }
            base.Draw(gameTime);
        }
    }
}