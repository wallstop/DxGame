using System;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Advanced
{
    public class MouseTrackingComponent : Component
    {
        private static readonly Lazy<MouseTrackingComponent> LazyInstance =
            new Lazy<MouseTrackingComponent>(() => new MouseTrackingComponent());

        public static MouseTrackingComponent Instance => LazyInstance.Value;

        private PhysicsComponent MousePhysics { get; }

        public bool Clicked { get; private set; }

        private bool ClickInProgress { get; set; }

        private MouseTrackingComponent()
        {
            Clicked = false;
            ClickInProgress = false;

            MousePhysics =
                PhysicsComponent.Builder()
                    .WithBounds(new DxVector2(50, 50))
                    .WithPosition(DxVector2.EmptyVector)
                    .WithCollidesWith(CollisionGroup.None)
                    .WithoutCollision()
                    .Build();
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            MousePhysics.Position = mouseState.Position;
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