﻿using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.State
{
    /* Will be called for a State's Draw method */

    public delegate void Presentation(SpriteBatch spriteBatch, DxGameTime gameTime);
}