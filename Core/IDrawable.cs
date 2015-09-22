using System;
using System.Collections.Generic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core
{
    public enum DrawPriority
    {
        INIT_SPRITEBATCH = -100,
        HIGHEST = -99,
        MAP = -80,
        HIGH = 1,
        NORMAL = 5,
        HUD_LAYER = 8,
        LOW = 10,
        END_SPRITEBATCH = 1000
    }

    public interface IDrawable : IComparable<IDrawable>
    {
        DrawPriority DrawPriority { get; }
        void Draw(SpriteBatch spriteBatch, DxGameTime gameTime);
    }

    public static class Drawable
    {
        public static IComparer<IDrawable> DefaultComparer { get; } = new LambdaUtils.LambdaComparer<IDrawable>(
            (first, second) =>
                ((int?) first?.DrawPriority ?? int.MinValue).CompareTo(((int?) second?.DrawPriority ?? int.MinValue)));
    }
}