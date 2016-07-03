using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
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

        protected override void Initialize()
        {
            base.Initialize();
            DxRectangle defaultBounds = new DxRectangle(0, 0, DxGame.Instance.Graphics.PreferredBackBufferWidth, DxGame.Instance.Graphics.PreferredBackBufferHeight);
            new UpdateWorldBounds(defaultBounds).Emit();
            new UpdateCameraBounds(defaultBounds).Emit();

            DeveloperModel devModel = new DeveloperModel();
            devModel.Create();
            GameObject playerPaddle = PaddleFactory.PlayerPaddle(new DxVector2(5, 10));
            playerPaddle.Create();

            PhysicsComponent paddlePhysics = playerPaddle.ComponentOfType<PhysicsComponent>();


            GameObject enemyPaddle = PaddleFactory.EnemyPaddle(new DxVector2(1220, 10));
            enemyPaddle.Create();

            PhysicsComponent enemyPaddlePhysics = enemyPaddle.ComponentOfType<PhysicsComponent>();


            GameObject ball =
                BallFactory.Ball(new DxVector2(Graphics.PreferredBackBufferWidth / 2.0,
                    Graphics.PreferredBackBufferHeight / 2.0));
            ball.Create();

            Force pushTowardsPlayer = new Force(new DxVector2(-100, 1));

            new PhysicsAttachment(pushTowardsPlayer, ball.Id).Emit();
        }

        protected override void LoadContent()
        {
            base.LoadContent();


        }

        private static bool COllideWithWorldNotBall(Fixture self, Fixture other, Contact contact)
        {
            return false;
        }
    }
}
