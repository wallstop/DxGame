﻿using System.Collections.Generic;
using DXGame.Core.Components.Basic;

namespace DXGame.Core.Components.Utils
{
    internal class DrawPriorityComparer : IComparer<DrawableComponent>
    {
        public int Compare(DrawableComponent lhs, DrawableComponent rhs)
        {
            return lhs.DrawPriority - rhs.DrawPriority;
        }
    }
}
