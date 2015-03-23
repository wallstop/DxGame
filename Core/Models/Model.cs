using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public abstract class Model : DrawableComponent
    {
        protected Model(DxGame game)
            : base(game)
        {
        }

        public override void Draw(DxGameTime gameTime)
        {
            // No-op in base
        }
    }
}
