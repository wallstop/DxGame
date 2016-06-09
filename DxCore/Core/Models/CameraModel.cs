using System;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Models
{
    public class CameraModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private const float MIN_SPEED = 1.5f;

        private const float IGNORE_THRESHOLD = 5f;

        public DxVector2 Position { get; private set; }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
            if(ReferenceEquals(playerModel, null))
            {
                return;
            }

            Player activePlayer = playerModel.ActivePlayer;
            if(ReferenceEquals(activePlayer, null))
            {
                return;
            }

            DxVector2 target = activePlayer.Position.Center;

            DxVector2 displacement = target - Position;
            float magnitude = displacement.Magnitude;
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
