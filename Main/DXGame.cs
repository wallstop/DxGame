#region Using Statements

using DXGame.Core.Simple;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace DXGame.Main
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class DXGame : Game
    {
        private const int height_ = 720;
        private const int width_ = 1280;
        private readonly SimpleMap map_;
        private readonly SimplePlayer player_;
        private GraphicsDeviceManager graphics_;
        private SpriteBatch spriteBatch_;

        public DXGame()
        {
            map_ = new SimpleMap("../../Content/Map/SimpleMap.txt");
            player_ = new SimplePlayer(map_);

            graphics_ = new GraphicsDeviceManager(this);
            graphics_.PreferredBackBufferHeight = height_;
            graphics_.PreferredBackBufferWidth = width_;

            Content.RootDirectory = "../../Content";
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            map_.Load(Content);
            player_.LoadContent(Content);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            player_.Update();

            // TODO: Add your update logic here

            // TODO: Fix update & draw lockstep so they're de-synced. Sync Update() to 60/30/whatever FPS, Draw() unlocked (or locked to player preference)

            // TODO (Short term): Have a dedicated InputSystem 
            /*
             * TODO (Long term): XNA/Monogame only supports polling-based input. In order to get this to the level of granularity that
             * I'd like, we need to have a dedicated InputSystem that is capable of short-polling (on the order of 1/10th of a millisecond) the keyboard/gamepad/mouse state, 
             * diffing the previous state, and publishing events if there is a change. Then it's a matter of hooking up subscribers to these events.
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
            spriteBatch_.Begin();

            map_.Draw(spriteBatch_);
            player_.Draw(spriteBatch_);

            spriteBatch_.End();
            base.Draw(gameTime);
        }
    }
}