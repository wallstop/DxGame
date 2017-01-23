using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Advanced.Position
{
    public class MouseTrackingComponent : Component, ISpatial
    {
        public bool Clicked { get; private set; }

        private bool ClickInProgress { get; set; }
        private ISpatial MouseSpatial { get; }

        public MouseTrackingComponent()
        {
            Clicked = false;
            ClickInProgress = false;

            MouseSpatial =
                SpatialComponent.UiTrackingBasedBuilder()
                    .WithUiOffsetProvider(() => Mouse.GetState().Position)
                    .WithDimensions(50, 50)
                    .Build();
        }

        public DxVector2 WorldCoordinates => MouseSpatial.WorldCoordinates;
        public DxRectangle Space => MouseSpatial.Space;

        protected override void Update(DxGameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
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