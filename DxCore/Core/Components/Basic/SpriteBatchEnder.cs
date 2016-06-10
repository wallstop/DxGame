using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Basic
{
    [Serializable]
    [DataContract]
    public class SpriteBatchEnder : DrawableComponent
    {
        public SpriteBatchEnder()
        {
            DrawPriority = DrawPriority.END_SPRITEBATCH;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.End();
        }
    }
}