using System;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Models
{
    public class CameraModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private Func<DxVector2> NoOpTarget => () => Position;

        private const float MIN_SPEED = 1.5f;

        private const float IGNORE_THRESHOLD = 5f;

        public DxVector2 Position { get; private set; }

        private Func<DxVector2> Target { get; set; }

        public CameraModel()
        {
            TrackActivePlayer();
        }

        public void TrackActivePlayer()
        {
            Target = () =>
            {
                PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
                if(ReferenceEquals(playerModel, null))
                {
                    return Position;
                }

                Player activePlayer = playerModel.ActivePlayer;
                if(ReferenceEquals(activePlayer, null))
                {
                    return Position;
                }

                DxVector2 target = activePlayer.Position.Center;
                return target;
            };
        }

        public void TrackPlayer(Player player)
        {
            Target = () => player.Position.Center;
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
