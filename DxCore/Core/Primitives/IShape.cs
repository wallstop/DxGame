using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Primitives
{
    public interface IShape
    {
        bool Contains(DxVector2 point);
        bool Intersects(DxRectangle box);
    }
}
