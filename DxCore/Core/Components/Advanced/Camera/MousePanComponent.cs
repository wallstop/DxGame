using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Advanced.Camera
{
    /**
        <summary>
            Moves the camera around when a mouse button (default middle) is continually pressed, tracking mouse movement 1:1
        </summary>

        <note>
            This should not be serialized, ever
        </note>
    */

    public sealed class MousePanComponent : DrawableComponent
    {
        private bool Enabled { get; set; }

        private MouseButton Enabler { get; } = MouseButton.Middle;
        private DxVector2 LastPosition { get; set; }

        public MousePanComponent()
        {
            Enabled = false;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(!Enabled)
            {
                return;
            }

            // Simple diffs
            DxVector2 currentPosition = Mouse.GetState().Position;
            DxVector2 delta = currentPosition - LastPosition;
            LastPosition = currentPosition;

            CameraService camera;
            if(DxGame.Instance.ServiceProvider.TryGet(out camera))
            {
                camera.MoveBy(delta.Inverse);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            bool previouslyEnabled = Enabled;
            InputService inputService;
            if(DxGame.Instance.ServiceProvider.TryGet(out inputService))
            {
                Enabled = inputService.InputHandler.CurrentMouseEvents.Any(mouseEvent => mouseEvent.Source == Enabler);
            }
            else
            {
                Enabled = false;
            }

            if(Enabled && !previouslyEnabled)
            {
                LastPosition = Mouse.GetState().Position;
            }
        }
    }
}