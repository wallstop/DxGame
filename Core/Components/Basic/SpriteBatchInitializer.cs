using System.Diagnostics;
using DXGame.Core.Components.Advanced;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    internal class SpriteBatchInitializer : DrawableComponent
    {
        private PositionalComponent playerPosition_;
        private BoundedSpatialComponent mainWindow_;

        // TODO: Think about BoundedSpatialComponent, can it be something else?
        public SpriteBatchInitializer(DxGame game, PositionalComponent playerPosition,
            BoundedSpatialComponent mainWindow)
            : base(game)
        {
            Debug.Assert(playerPosition != null,
                "SpriteBatchInitializer cannot be initialized with a null player position component");
            Debug.Assert(mainWindow != null,
                "SpriteBatchInitializer cannot be initialized with a null main window component");
            playerPosition_ = playerPosition;
            mainWindow_ = mainWindow;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch_.Begin();
            // TODO: Store screen info & ref to player position here, do transform
        }
    }
}