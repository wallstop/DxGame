using System.Collections.Generic;
using DXGame.Core.Components.Basic;

namespace DXGame.Core.Components.Utils
{
    public class UpdatePriorityComparer : IComparer<Component>
    {
        public int Compare(Component lhs, Component rhs)
        {
            return lhs.UpdatePriority - rhs.UpdatePriority;
        }
    }
}