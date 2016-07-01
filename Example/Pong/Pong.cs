using DxCore;
using DxCore.Core;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using Pong.Core.Generators;

namespace Pong
{
    public class Pong : DxGame
    {
        public Pong()
        {
            // IMPLEMENTED ///////////////////

            /* One paddle player controlled */
            /* One paddle moving randomly */
            /* A ball */
            /* A ball kicker-offer */

            // TODO //////////////////////////

            /* Goal detection */
            /* Ball reset after goal */
            /* Score tracking */
        }

        protected override void SetUp()
        {
            // NO-OP LOL, we're not special at all
            // TODO: Remove this necessity?
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            DeveloperModel devModel = new DeveloperModel();
            devModel.Create();
            GameObject playerPaddle = PaddleFactory.PlayerPaddle(new DxVector2(0, 0));
            playerPaddle.Create();

            GameObject enemyPaddle = PaddleFactory.EnemyPaddle(new DxVector2(1230, 0));
            enemyPaddle.Create();

            GameObject ball =
                BallFactory.Ball(new DxVector2(Graphics.PreferredBackBufferWidth / 2.0,
                    Graphics.PreferredBackBufferHeight / 2.0));
            ball.Create();

            Impulse pushTowardsPlayer = new Impulse(new DxVector2(-100000, 100));

            new PhysicsAttachment(pushTowardsPlayer, ball.Id).Emit();
        }
    }
}
