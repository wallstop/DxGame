using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using MapEditorLibrary.Controls;
using Microsoft.Xna.Framework.Graphics;
using Model = DxCore.Core.Models.Model;

namespace MapEditorLibrary.Core.Models
{
    public class RootUiModel : Model
    {
        public Root UI { get; }

        public RootUiModel(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            DrawPriority = DrawPriority.MenuLayer;
            UI = rootUi;
            UI.DataContext = new AssetManagerView();
        }

        public override void LoadContent()
        {
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Segoe_UI_15_Bold");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);

            // I guess we bind controls here?
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
