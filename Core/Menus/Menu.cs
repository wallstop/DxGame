using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableComponent
    {
        private readonly List<MenuItem> menuItems_ = new List<MenuItem>();
        private readonly GameObject mousePointer_;

        protected List<MenuItem> MenuItems
        {
            get { return menuItems_; }
        }

        protected Menu(DxGame game) : base(game)
        {
            var mousePosition = new MouseTrackingComponent(DxGame).WithPosition(0, 0);
            var mouseSprite = new SimpleSpriteComponent(DxGame).WithAsset("MousePointer").WithPosition(mousePosition);
            mousePointer_ = new GameObject().WithComponents(mousePosition, mouseSprite);
            DxGame.AddAndInitializeComponents(mousePosition, mouseSprite);
        }

        public override void Draw(DxGameTime gameTime)
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                spriteBatch_.DrawString(menuItem.SpriteFont, menuItem.Text, menuItem.Space.XY(), Color.DeepPink);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            var mousePosition = mousePointer_.ComponentOfType<MouseTrackingComponent>();
            var mouseSprite = mousePointer_.ComponentOfType<SimpleSpriteComponent>();
            if (mousePosition.Clicked)
            {
                Point center = mouseSprite.BoundingBox.Center;
                // We need to translate the center of the sprite's bounding box to where the mouse currently is.
                DxVector2 mouseXY = mousePosition.Position;
                center.X += (int) mouseXY.X;
                center.Y += (int) mouseXY.Y;

                MenuItem clickedMenuItem = MenuItems.FirstOrDefault(menuItem => menuItem.Space.Contains(center));
                if (clickedMenuItem != null)
                {
                    clickedMenuItem.OnAction();
                }
            }

            base.Update(gameTime);
        }

        public override void Remove()
        {
            DxGame.RemoveGameObject(mousePointer_);
            base.Remove();
        }
    }
}