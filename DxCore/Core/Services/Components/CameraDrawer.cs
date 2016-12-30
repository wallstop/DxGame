using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Services.Components
{
    public sealed class CameraDrawer : DrawableComponent
    {
        private const float MinSpeed = 1.5f;
        private const float IgnoreThreshold = 5;

        private CameraService CameraService { get; }

        public CameraDrawer(CameraService cameraService)
        {
            DrawPriority = DrawPriority.InitSpritebatch;
            Validate.Hard.IsNotNull(cameraService);
            CameraService = cameraService;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            UpdatePosition(gameTime);

            // TODO: Change this ugly color
            DxGame.Instance.GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Stop calling ourselves referentially and shit
            // TODO: Clean up
            DxRectangle screen = DxGame.Instance.ScreenRegion;
            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);
            Matrix scaled = Matrix.CreateScale(CameraService.ZoomAmount);
            cameraShift *= scaled;
            CameraService.Transform = cameraShift;

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
                CameraService.Transform);
        }

        private void UpdatePosition(DxGameTime gameTime)
        {
            DxVector2 target = CameraService.Target.Invoke();

            DxVector2 displacement = target - CameraService.Position;
            float magnitude = displacement.Magnitude;
            // TODO: Scale smoother
            if(magnitude < IgnoreThreshold)
            {
                return;
            }

            double scalar = magnitude / (DxGame.Instance.TargetFps / 8);
            scalar = Math.Max(scalar, MinSpeed);
            DxVector2 adjustment = displacement.UnitVector * scalar * gameTime.ScaleFactor;
            CameraService.Position += adjustment;
        }
    }
}