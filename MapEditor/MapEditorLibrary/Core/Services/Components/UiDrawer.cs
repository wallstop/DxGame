using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Services.Components
{
    public sealed class UiDrawer : DrawableComponent
    {
        private Root UI { get; }

        public UiDrawer(Root ui)
        {
            DrawPriority = DrawPriority.MenuLayer;
            Validate.Hard.IsNotNull(ui);
            UI = ui;
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
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Fonts/visitor_tt1_brk_15_Regular");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);
        }

        protected override void Update(DxGameTime gameTime)
        {
            UI.UpdateInput(EmptyKeysGameTime(gameTime));
            UI.UpdateLayout(EmptyKeysGameTime(gameTime));
            base.Update(gameTime);
        }
    }
}