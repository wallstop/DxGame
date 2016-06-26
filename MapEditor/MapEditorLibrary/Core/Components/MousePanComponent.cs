using System.Linq;
using DxCore;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapEditorLibrary.Core.Components
{
    /**
        <summary>
            Moves the camera around when a mouse button (default middle) is continually pressed, tracking mouse movement 1:1
        </summary>
    */
    public class MousePanComponent : DrawableComponent
    {
        private DxVector2 LastPosition { get; set; }

        private MouseButton Enabler { get; } = MouseButton.Middle;

        private bool Enabled { get; set; }

        public MousePanComponent()
        {
            Enabled = false;
        }

        protected override void Update(DxGameTime gameTime)
        {
            bool previouslyEnabled = Enabled;
            Enabled =
                DxGame.Instance.Model<InputModel>()
                    .InputHandler.CurrentMouseEvents.Any(mouseEvent => mouseEvent.Source == Enabler);
            if(Enabled && !previouslyEnabled)
            {
                LastPosition = Mouse.GetState().Position;
            }
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

            CameraModel camera = DxGame.Instance.Model<CameraModel>();
            camera.MoveBy(delta.Inverse);
        }
    }
}
