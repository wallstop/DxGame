using System;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Models
{
    public class CameraModel : Model
    {
        private Func<DxVector2> NoOpTarget => () => Position;

        private const float MIN_SPEED = 1.5f;

        private const float IGNORE_THRESHOLD = 5f;

        private DxVector2 position_;
        private DxRectangle bounds_;

        /* Where the camera should be looking at */
        public DxVector2 Position
        {
            get { return position_; }
            set
            {
                position_ = value.ClampTo(Bounds);
            }
        }

        // TODO: Bound position 
        public DxRectangle Bounds {
            get { return bounds_; }
            set
            {
                DxRectangle screen = DxGame.Instance.Graphics.Bounds();
                float xOffset = screen.Width / 2;
                float yOffset = screen.Height / 2;
                bounds_ = new DxRectangle(value.X + xOffset, value.Y + yOffset, Math.Max(0, value.Width - screen.Width), Math.Max(0, value.Height - screen.Height));
                /* Snap position to our new bounds */
                Position = Position;
            }
        }

        private Func<DxVector2> Target { get; set; }

        public CameraModel()
        {
            Bounds = DxGame.Instance.Screen;
            TrackActivePlayer();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<UpdateCameraBounds>(HandleUpdateCameraBounds);
            base.OnAttach();
        }

        private void HandleUpdateCameraBounds(UpdateCameraBounds cameraBounds)
        {
            Bounds = cameraBounds.Bounds;
        }

        public void TrackActivePlayer()
        {
            TrackPlayer(DxGame.Instance.Model<PlayerModel>()?.ActivePlayer);
        }

        public void TrackPlayer(Player player)
        {
            Target = () => player?.Position.Center ?? Position;
        }

        public void MoveTo(DxVector2 target)
        {
            Target = () => target;
        }

        public void MoveBy(DxVector2 translation)
        {
            Position += translation;
        }

        public void SnapTo(DxVector2 target)
        {
            Position = target;
            Target = NoOpTarget;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            DxVector2 target = Target.Invoke();

            DxVector2 displacement = target - Position;
            float magnitude = displacement.Magnitude;
            // TODO: Scale smoother
            if(magnitude < IGNORE_THRESHOLD)
            {
                return;
            }

            double scalar = magnitude / (DxGame.Instance.TargetFps / 8);
            scalar = Math.Max(scalar, MIN_SPEED);
            DxVector2 adjustment = displacement.UnitVector * scalar * gameTime.ScaleFactor;
            Position += adjustment;
        }
    }
}
