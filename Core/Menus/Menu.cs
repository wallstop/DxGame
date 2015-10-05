using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableComponent
    {
        private readonly GameObject mousePointer_;
        protected List<MenuItem> MenuItems { get; } = new List<MenuItem>();

        protected Menu(DxGame game) : base(game)
        {
            var mousePosition = new MouseTrackingComponent(DxGame);
            var mouseSprite = new SimpleSpriteComponent(DxGame).WithAsset("MousePointer").WithPosition(mousePosition);
            mousePointer_ = GameObject.Builder().WithComponents(mousePosition, mouseSprite).Build();
            DxGame.AddAndInitializeComponents(mousePosition, mouseSprite);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                spriteBatch.DrawString(menuItem.SpriteFont, menuItem.Text, menuItem.Space.XY(), Color.DeepPink);
            }
        }

        public override bool ShouldSerialize => false;

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
                clickedMenuItem?.OnAction();
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