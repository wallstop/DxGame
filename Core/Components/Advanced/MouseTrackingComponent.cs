using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    public class MouseTrackingComponent : PositionalComponent
    {
        public bool Clicked { get; private set; }

        public MouseTrackingComponent(DxGame game) : base(game)
        {
            Clicked = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            Position = new Vector2(mouseState.Position.X, mouseState.Position.Y);
            Clicked = mouseState.LeftButton == ButtonState.Pressed; // Only check left button for now. We can enhance this later.
        }
    }
}