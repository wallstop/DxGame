using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public abstract class Model
    {
        protected DxGame DxGame;

        protected Model(DxGame game)
        {
            DxGame = game;
        }

        public virtual void Initialize()
        {
            // No-op in base
        }

        public virtual void Update(DxGameTime gameTime)
        {
            // No-op in base
        }

        public virtual void Draw(DxGameTime gameTime)
        {
            // No-op in base
        }
    }
}
