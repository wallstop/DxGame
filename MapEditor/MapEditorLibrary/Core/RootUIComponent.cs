using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditorLibrary.Core
{
    public class RootUiComponent : DrawableComponent
    {
        private Root UI { get; }

        public RootUiComponent(Root rootUi)
        {
            Validate.IsNotNullOrDefault(rootUi);
            UI = rootUi;
        }

        protected override void Update(DxGameTime gameTime)
        {
            UI.UpdateInput(EmptyKeysGameTime(gameTime));
            UI.UpdateLayout(EmptyKeysGameTime(gameTime));
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            UI.Draw(EmptyKeysGameTime(gameTime));
        }

        public static double EmptyKeysGameTime(DxGameTime gameTime)
        {
            return gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
