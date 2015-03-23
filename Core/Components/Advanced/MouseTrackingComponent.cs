using System;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class MouseTrackingComponent : PositionalComponent
    {
        [DataMember]
        public bool Clicked { get; private set; }

        [DataMember]
        private bool ClickInProgress { get; set; }

        public MouseTrackingComponent(DxGame game) : base(game)
        {
            Clicked = false;
            ClickInProgress = false;
        }

        public override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            Position = new DxVector2(mouseState.Position.X, mouseState.Position.Y);
            if (ClickInProgress)
            {
                // Only check left button for now. We can enhance this later.
                Clicked = mouseState.LeftButton == ButtonState.Released;
            }
            ClickInProgress = mouseState.LeftButton == ButtonState.Pressed;
        }
    }
}