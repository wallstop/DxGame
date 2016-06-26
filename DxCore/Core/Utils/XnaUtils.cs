using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Utils
{
    public static class XnaUtils
    {
        public static DxRectangle Bounds(this GraphicsDeviceManager graphics)
        {
            return new DxRectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
    }
}
