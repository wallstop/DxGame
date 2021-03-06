﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Basic
{
    [Serializable]
    [DataContract]
    public sealed class SpriteBatchEnder : DrawableComponent
    {
        private static readonly Lazy<SpriteBatchEnder> Singleton =
            new Lazy<SpriteBatchEnder>(() => new SpriteBatchEnder());

        public static SpriteBatchEnder Instance => Singleton.Value;

        private SpriteBatchEnder()
        {
            DrawPriority = DrawPriority.EndSpritebatch;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.End();
        }
    }
}