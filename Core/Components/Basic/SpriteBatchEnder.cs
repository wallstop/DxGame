using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    internal class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.END_SPRITEBATCH;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch_.End();
        }

        public override void Write(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void Read(NetIncomingMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}