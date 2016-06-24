using System;
using DxCore;
using DxCore.Core;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Controls;
using EmptyKeys.UserInterface.Controls.Primitives;
using EmptyKeys.UserInterface.Generated;
using EmptyKeys.UserInterface.Input;
using MapEditorLibrary.Controls;
using Microsoft.Xna.Framework.Graphics;
using Model = DxCore.Core.Models.Model;

namespace MapEditorLibrary.Core.Models
{
    public class RootUiModel : Model
    {
        public Root UI { get; }

        private AssetManagerView AssetManagerView { get; }

        public RootUiModel(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            DrawPriority = DrawPriority.MenuLayer;
            UI = rootUi;
            AssetManagerView = new AssetManagerView();
            UI.DataContext = AssetManagerView;
        }

        public override void LoadContent()
        {
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Segoe_UI_15_Bold");
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
