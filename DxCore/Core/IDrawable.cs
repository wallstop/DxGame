using System;
using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core
{
    public enum DrawPriority
    {
        InitSpritebatch = -100,
        Highest = -99,
        Map = -80,
        High = 1,
        Normal = 5,
        HudLayer = 8,
        Low = 10,
        Lowest = 50,
        EndSpritebatch = 1000,
        /* 
            User primitives (actual GPU shader calls) must be made after sprite batch has finished 
            (http://stackoverflow.com/questions/27431038/going-back-to-spritebatch-draw-after-using-graphicsdevice-drawuserprimitives) 
        */
        UserPrimitives = 1001,
        MenuLayer = 1002
    }

    public interface IDrawable : IComparable<IDrawable>
    {
        DrawPriority DrawPriority { get; }
        void Draw(SpriteBatch spriteBatch, DxGameTime gameTime);
    }

    public static class Drawable
    {
        public static IComparer<IDrawable> DefaultComparer { get; } =
            new LambdaUtils.LambdaComparer<IDrawable>(
                (first, second) =>
                    ((int?) first?.DrawPriority ?? int.MinValue).CompareTo((int?) second?.DrawPriority ?? int.MinValue))
            ;
    }
}