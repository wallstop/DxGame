using System.Collections.Generic;
using System.Linq;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Menus
{
    public abstract class Menu : DrawableComponent
    {
        private readonly GameObject mousePointer_;
        protected List<MenuItem> MenuItems { get; } = new List<MenuItem>();

        protected Menu()
        {
            MouseTrackingComponent mousePosition = new MouseTrackingComponent();
            SimpleSpriteComponent mouseSprite =
                SimpleSpriteComponent.Builder().WithAsset("MousePointer").WithSpatial(mousePosition).Build();
            mousePointer_ = GameObject.Builder().WithComponents(mousePosition, mouseSprite).Build();

            mousePointer_.Create();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach(MenuItem menuItem in MenuItems)
            {
                spriteBatch.DrawString(menuItem.SpriteFont, menuItem.Text, menuItem.Space.XY(), Color.DeepPink);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            MouseTrackingComponent mousePosition = mousePointer_.ComponentOfType<MouseTrackingComponent>();
            SimpleSpriteComponent mouseSprite = mousePointer_.ComponentOfType<SimpleSpriteComponent>();
            if(mousePosition.Clicked)
            {
                DxVector2 center = mouseSprite.Space.Center;
                MenuItem clickedMenuItem = MenuItems.FirstOrDefault(menuItem => menuItem.Space.Contains(center));
                clickedMenuItem?.OnAction();
            }
        }

        public override void Remove()
        {
            mousePointer_.Remove();
            foreach(MenuItem menuItem in MenuItems.ToList())
            {
                menuItem.Remove();
            }
            base.Remove();
        }
    }
}