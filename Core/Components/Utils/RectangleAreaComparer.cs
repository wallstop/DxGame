using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Utils
{
    public class RectangleAreaComparer : IComparer<Rectangle>
    {
        public int Compare(Rectangle lhs, Rectangle rhs)
        {
            return Math.Abs(lhs.Width * lhs.Height) - Math.Abs(rhs.Width * rhs.Height);
        }
    }
}