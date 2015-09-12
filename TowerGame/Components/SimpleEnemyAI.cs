using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.TowerGame.Components
{
    public class SimpleEnemyAI : Component
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(2);
        private TimeSpan lastChangedMovement_ = TimeSpan.FromSeconds(0);
        private Direction direction_ = Direction.East;

        public SimpleEnemyAI(DxGame game) 
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            if (lastChangedMovement_ + MOVEMENT_DELAY_CHECK < gameTime.TotalGameTime)
            {
                Random rGen = new Random();
                direction_ = rGen.Next() % 2 == 0 ? Direction.East : Direction.West;
                lastChangedMovement_ = gameTime.TotalGameTime;
            }
            var movementRequest = new MovementRequest {Direction = direction_};
            Parent.BroadcastMessage(movementRequest);
            base.Update(gameTime);
        }
    }
}
