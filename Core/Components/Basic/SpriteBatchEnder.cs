using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;
using ProtoBuf;

namespace DXGame.Core.Components.Basic
{
    [Serializable]
    [DataContract]
    [ProtoContract]
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