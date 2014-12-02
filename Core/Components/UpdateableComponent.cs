using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public abstract class UpdateableComponent : Component
    {
        protected UpdateableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        public abstract bool Update(GameTime gameTime);
    }
}