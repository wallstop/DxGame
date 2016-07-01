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
                SimpleSpriteComponent.Builder().WithAsset("MousePointer").WithPosition(mousePosition).Build();
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
            var mousePosition = mousePointer_.ComponentOfType<MouseTrackingComponent>();
            var mouseSprite = mousePointer_.ComponentOfType<SimpleSpriteComponent>();
            if(mousePosition.Clicked)
            {
                Point center = mouseSprite.BoundingBox.Center;
                // We need to translate the center of the sprite's bounding box to where the mouse currently is.
                DxVector2 mouseXY = mousePosition.Position;
                center.X += (int) mouseXY.X;
                center.Y += (int) mouseXY.Y;

                MenuItem clickedMenuItem = MenuItems.FirstOrDefault(menuItem => menuItem.Space.Contains(center));
                clickedMenuItem?.OnAction();
            }

            base.Update(gameTime);
        }

        public override void Remove()
        {
            mousePointer_.Remove();
            base.Remove();
        }
    }
}