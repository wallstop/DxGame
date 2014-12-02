using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components
{
    public abstract class DrawableComponent : Component
    {
        protected DrawableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        public abstract bool LoadContent(ContentManager contentManager);

        public abstract bool Draw(SpriteBatch spriteBatch);
    }
}