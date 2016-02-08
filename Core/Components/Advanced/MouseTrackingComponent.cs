using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Input;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class MouseTrackingComponent : PositionalComponent
    {
        [ProtoMember(1)]
        [DataMember]
        public bool Clicked { get; private set; }

        [ProtoMember(2)]
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