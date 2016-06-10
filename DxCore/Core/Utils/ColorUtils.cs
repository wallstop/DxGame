using Microsoft.Xna.Framework;

namespace DxCore.Core.Utils
{
    /**
        Extention methods for XNA Color classes
    */
    public static class ColorUtils
    {

        public static Color Transparent(float alphaBlend)
        {
            return new Color(Color.White.ToVector4() * alphaBlend);
        }
    }
}
