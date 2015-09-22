using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Basic
{
    [Serializable]
    [DataContract]
    public class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.END_SPRITEBATCH;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.End();
        }
    }
}