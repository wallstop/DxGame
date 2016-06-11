using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Utils
{
    public static class XnaUtils
    {
        public static DxVector2 ToDxVector2(this Point point)
        {
            return new DxVector2(point.X, point.Y);
        }
    }
}
