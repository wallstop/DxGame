using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class MouseTrackingComponent : PositionalComponent
    {
        [DataMember]
        public bool Clicked { get; private set; }

        [DataMember]
        private bool ClickInProgress { get; set; }

        public MouseTrackingComponent() : base(new DxVector2())
        {
            Clicked = false;
            ClickInProgress = false;
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            Position = new DxVector2(mouseState.Position.X, mouseState.Position.Y);
            if(ClickInProgress)
            {
                // Only check left button for now. We can enhance this later.
                Clicked = mouseState.LeftButton == ButtonState.Released;
            }
            else
            {
                Clicked = false;
            }
            ClickInProgress = mouseState.LeftButton == ButtonState.Pressed;
        }
    }
}