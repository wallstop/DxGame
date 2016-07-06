using DxCore;
using DxCore.Core;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework.Graphics;
using Model = DxCore.Core.Models.Model;

namespace BabelUILibrary.Core.Models
{
    public class UiModel : Model
    {
        public Root UI { get; }

        public UiModel(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            DrawPriority = DrawPriority.MenuLayer;
            UI = rootUi;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            UI.Draw(EmptyKeysGameTime(gameTime));
        }

        public static double EmptyKeysGameTime(DxGameTime gameTime)
        {
            return gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void LoadContent()
        {
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);

            // I guess we bind controls here?

            // TODO: We don't really want drag-drop. What we really want is double click to select
        }

        protected override void Update(DxGameTime gameTime)
        {
            UI.UpdateInput(EmptyKeysGameTime(gameTime));
            UI.UpdateLayout(EmptyKeysGameTime(gameTime));
            base.Update(gameTime);
        }
    }
}
