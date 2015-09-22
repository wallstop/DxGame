using System;
using System.Collections.Generic;
using DXGame.Core.Primitives;

namespace DXGame.Core.Components.Utils
{
    public class DxRectangleAreaComparer : IComparer<DxRectangle>
    {
        public int Compare(DxRectangle x, DxRectangle y)
        {
            return (int) Math.Floor(x.Area - y.Area);
        }
    }
}