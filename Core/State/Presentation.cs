using DXGame.Core.Wrappers;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.State
{
    /* Will be called for a State's Draw method */

    public delegate void Presentation(SpriteBatch spriteBatch, DxGameTime gameTime);
}