﻿using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    public class SpriteBatchInitializer : DrawableComponent
    {
        public SpriteBatchInitializer(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.INIT_SPRITEBATCH;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Rectangle2f screen = DxGame.ScreenRegion;

            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);
            spriteBatch_.Begin(0, null, null, null, null, null, cameraShift);
        }

        public override void SerializeTo(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void DeserializeFrom(NetIncomingMessage messsage)
        {
            throw new System.NotImplementedException();
        }
    }
}