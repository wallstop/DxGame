using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
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
