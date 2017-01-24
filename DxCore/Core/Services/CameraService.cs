using System;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Camera;
using DxCore.Core.Primitives;
using DxCore.Core.Services.Components;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    public class CameraService : DxService
    {
        private const float MinZoom = 1 / 3.0f;
        private const float MaxZoom = 3;
        private DxRectangle bounds_;

        private DxVector2 position_;
        private float zoomAmount_;

        public DxRectangle Bounds
        {
            get { return bounds_; }
            set
            {
                // TODO: Offload
                DxVector2 screenCenter = DxGame.Instance.Graphics.Bounds().Center;
                bounds_ = new DxRectangle(value.X + screenCenter.X, value.Y + screenCenter.Y,
                    Math.Max(0, value.Width - screenCenter.X * 2), Math.Max(0, value.Height - screenCenter.Y * 2));
                /* Snap position to our newly created bounds */
                Position = Position;
            }
        }

        /**
            Where the Camera is looking (Point target, center of screen)
        */

        public DxVector2 Position
        {
            get { return position_; }
            set { position_ = value.ClampTo(Bounds); }
        }

        public Func<DxVector2> Target { get; set; }

        public Matrix Transform { get; set; } = Matrix.Identity;

        public float ZoomAmount
        {
            get { return zoomAmount_; }
            set { ClampAndSetZoomAmount(value); }
        }

        private CameraDrawer CameraDrawer { get; set; }

        private Func<DxVector2> NoOpTarget => () => Position;

        public CameraService()
        {
            DxVector2 screenDimensions = DxGame.Instance.GameSettings.VideoSettings.ScreenDimensions;
            Bounds = new DxRectangle(0, 0, screenDimensions.X, screenDimensions.Y);
            // TODO:  Grab from sprite batch init?
            zoomAmount_ = 1.0f;
            bounds_ = new DxRectangle(0, 0, DxGame.Instance.Graphics.PreferredBackBufferWidth,
                DxGame.Instance.Graphics.PreferredBackBufferHeight);
            FollowActivePlayer();
        }

        /* TODO: Make this delegate serializable? */

        public void Follow(Func<DxVector2> pointToTrack)
        {
            Target = pointToTrack;
        }

        public void FollowActivePlayer()
        {
            Follow(() => DxGame.Instance.Service<PlayerService>()?.ActivePlayer?.Position.Center ?? Position);
        }

        public void MoveBy(DxVector2 translation)
        {
            Position += translation;
        }

        public void MoveTo(DxVector2 target)
        {
            Target = () => target;
        }

        public void SnapTo(DxVector2 target)
        {
            Position = target;
            Target = NoOpTarget;
        }

        public DxVector2 UiOffsetToWorldCoordinates(DxVector2 nonScaledUiPoint)
        {
            return Vector2.Transform(nonScaledUiPoint.Vector2, Matrix.Invert(Transform));
        }

        public DxVector2 WorldCoordinatesToUiOffset(DxVector2 worldCoordinate)
        {
            return Vector2.Transform(worldCoordinate.Vector2, Transform);
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(CameraDrawer))
            {
                CameraDrawer = new CameraDrawer(this);
                Self.AttachComponent(CameraDrawer);
            }

            Self.MessageHandler.RegisterMessageHandler<UpdateCameraBounds>(HandleUpdateCameraBounds);
            Self.MessageHandler.RegisterMessageHandler<ZoomRequest>(HandleZoomRequest);
        }

        private void ClampAndSetZoomAmount(float value)
        {
            zoomAmount_ = MathHelper.Clamp(value, MinZoom, MaxZoom);
        }

        private void HandleUpdateCameraBounds(UpdateCameraBounds cameraBounds)
        {
            Bounds = cameraBounds.Bounds;
        }

        private void HandleZoomRequest(ZoomRequest zoomRequest)
        {
            /* Maybe we weren't the authors - we want to handle that anyways */
            ZoomAmount = zoomRequest.ZoomLevel;
        }
    }
}