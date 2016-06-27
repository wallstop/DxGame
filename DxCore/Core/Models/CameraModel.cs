using System;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Camera;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Models
{
    public class CameraModel : Model
    {
        private Func<DxVector2> NoOpTarget => () => Position;

        private const float MinSpeed = 1.5f;
        private const float IgnoreThreshold = 5f;

        private const float MinZoom = 0.3f;
        private const float MaxZoom = 500f;

        private DxVector2 position_;
        private DxRectangle bounds_;
        private float zoomAmount_;

        public DxVector2 OffsetFromOrigin => Position - DxGame.Instance.Graphics.Bounds().Center;

        /**
            Where the Camera is looking (Point target, center of screen)
        */

        public DxVector2 Position
        {
            get { return position_; }
            set { position_ = value.ClampTo(Bounds); }
        }

        public DxRectangle Bounds
        {
            get { return bounds_; }
            set
            {
                DxVector2 screenCenter = DxGame.Instance.Graphics.Bounds().Center;
                bounds_ = new DxRectangle(value.X + screenCenter.X, value.Y + screenCenter.Y,
                    Math.Max(0, value.Width - screenCenter.X * 2), Math.Max(0, value.Height - screenCenter.Y * 2));
                /* Snap position to our newly created bounds */
                Position = Position;
            }
        }

        public Matrix Transform { get; private set; } = Matrix.Identity;

        private Func<DxVector2> Target { get; set; }

        public CameraModel()
        {
            Bounds = DxGame.Instance.Screen;
            DrawPriority = DrawPriority.InitSpritebatch;
            // TODO:  Grab from sprite batch init?
            zoomAmount_ = 1.0f;
            TrackActivePlayer();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<UpdateCameraBounds>(HandleUpdateCameraBounds);
            RegisterMessageHandler<ZoomRequest>(HandleZoomRequest);
            base.OnAttach();
        }

        private void HandleUpdateCameraBounds(UpdateCameraBounds cameraBounds)
        {
            Bounds = cameraBounds.Bounds;
        }

        private void HandleZoomRequest(ZoomRequest zoomRequest)
        {
            /* Maybe we weren't the authors - we want to handle that anyways */
            ClampAndSetZoomAmount(zoomRequest.ZoomLevel);
        }

        private void ClampAndSetZoomAmount(float value)
        {
            zoomAmount_ = MathHelper.Clamp(value, MinZoom, MaxZoom);
        }

        public float ZoomAmount
        {
            get { return zoomAmount_; }
            set
            {
                ClampAndSetZoomAmount(value);
            }
        }

        public void TrackActivePlayer()
        {
            TrackPlayer(DxGame.Instance.Model<PlayerModel>()?.ActivePlayer);
        }

        /* TODO: Make this delegate serializable? */

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

        private void UpdatePosition(DxGameTime gameTime)
        {
            DxVector2 target = Target.Invoke();

            DxVector2 displacement = target - Position;
            float magnitude = displacement.Magnitude;
            // TODO: Scale smoother
            if(magnitude < IgnoreThreshold)
            {
                return;
            }

            double scalar = magnitude / (DxGame.Instance.TargetFps / 8);
            scalar = Math.Max(scalar, MinSpeed);
            DxVector2 adjustment = displacement.UnitVector * scalar * gameTime.ScaleFactor;
            Position += adjustment;
        }

        public DxVector2 Transformed(DxVector2 nonScaledUiPoint)
        {
            return Vector2.Transform(nonScaledUiPoint.Vector2, Transform);
        }

        public DxVector2 Invert(DxVector2 nonScaledUiPoint)
        {
            return Vector2.Transform(nonScaledUiPoint.Vector2, Matrix.Invert(Transform));
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            UpdatePosition(gameTime);

            // TODO: Change this ugly color
            DxGame.Instance.GraphicsDevice.Clear(Color.DarkGray);

            DxRectangle screen = DxGame.Instance.ScreenRegion;
            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);
            Matrix scaled = Matrix.CreateScale(ZoomAmount);
            cameraShift *= scaled;
            Transform = cameraShift;

            /*
                http://gamedev.stackexchange.com/questions/19761/in-xna-3-1-is-it-possible-to-disable-texture-filtering
                http://gamedev.stackexchange.com/questions/6820/how-do-i-disable-texture-filtering-for-sprite-scaling-in-xna-4-0
                http://stackoverflow.com/questions/8130149/how-to-set-xnas-texturefilter-to-point

                We don't want *ANY* smoothing happening with any of our sprites. This causes "nice" antialiasing, which for us, is shit.
                We're aiming for a "pixel art" kind of game, not a "shitty psuedo-pixel art" kind of game.

                Assigning SamplerState to PointClamp mode here allows us to preserve pixel-perfect clarity while scaling images both up
                and down, giving us that sweet pixel look that we're aiming for.
            */
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                Transform);
        }
    }
}
