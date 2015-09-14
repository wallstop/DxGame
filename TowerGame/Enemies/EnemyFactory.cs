using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Messaging;
using DXGame.Core.State;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Action = DXGame.Core.State.Action;

namespace DXGame.TowerGame.Enemies
{
    public static class EnemyFactory
    {
        private static readonly float INITIAL_MOVEMENT_BOOST = 1.1f;

        public static StateMachine SimpleBoxBehavior(DxGame game,
            AnimationComponent.AnimationComponentBuilder animationBuilder, GameObject enemy)
        {
            var enemyName = "Box";
            var simple = "Simple";
            //var angry = "Angry";
            var stateMachineBuilder = StateMachine.Builder();
            stateMachineBuilder.WithDxGame(game);

            // Construct states and use them to build animations
            var idleState = IdleState(enemy);
            animationBuilder.WithStateAndAsset(idleState, AnimationFactory.AnimationFor(enemyName, simple));
            var stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();

            var moveLeftState = MoveLeftState(enemy);
            animationBuilder.WithStateAndAsset(moveLeftState, AnimationFactory.AnimationFor(enemyName, simple));
            var moveRightState = MoveRightState(enemy);
            animationBuilder.WithStateAndAsset(moveRightState, AnimationFactory.AnimationFor(enemyName, simple));

            // Construct and attach triggers
            Trigger moveLeftTrigger = (enemyInstance, gameTime) =>
            {
                var moveRequests = enemyInstance.CurrentMessages
                    .OfType<MovementRequest>();
                return moveRequests.Any(request => request.Direction == Direction.West);
            };
            var moveLeftTransition = new Transition(moveLeftTrigger, moveLeftState);

            idleState.Transitions.Add(moveLeftTransition);
            moveLeftState.Transitions.Add(moveLeftTransition);
            moveRightState.Transitions.Add(moveLeftTransition);

            Trigger moveRightTrigger = (enemyInstance, gameTime) => 
            {
                var moveRequests = enemyInstance.CurrentMessages
                    .OfType<MovementRequest>();
                return moveRequests.Any(request => request.Direction == Direction.East);
            };
            var moveRightTransition = new Transition(moveRightTrigger, moveRightState);
            idleState.Transitions.Add(moveRightTransition);
            moveLeftState.Transitions.Add(moveRightTransition);
            moveRightState.Transitions.Add(moveRightTransition);

            Trigger returnToIdleTrigger = (enemyInstance, gameTime) =>
            {
                var moveRequests = enemyInstance.CurrentMessages
                    .OfType<MovementRequest>();
                return moveRequests.All(request => request.Direction != Direction.West && request.Direction != Direction.East);
            };

            var returnToIdleTransition = new Transition(returnToIdleTrigger, idleState);
            idleState.Transitions.Add(returnToIdleTransition);
            moveLeftState.Transitions.Add(returnToIdleTransition);
            moveRightState.Transitions.Add(returnToIdleTransition);

            return stateMachine;
        }

        /* TODO: Refactor this and PlayerBuilder, there are some trivially mergable functions. Also, come up with better way to create StateMachines */
        private static State IdleState(GameObject enemy)
        {
            return State.Builder().WithName("Idle").WithAction(gameTime =>
            {
                var existingVelocity = enemy.ComponentOfType<PhysicsComponent>().Velocity;
                enemy.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(0, existingVelocity.Y);
            }).Build();
        }

        private static State MoveLeftState(GameObject enemy)
        {
            return
                State.Builder()
                    .WithName("Moving Left")
                    .WithAction(MoveLeftAction(enemy))
                    .Build();
        }

        private static Action MoveLeftAction(GameObject enemy)
        {
            /* TODO: Configify */
            var moveSpeed = 5;
            return gameTime =>
            {
                if (enemy.ComponentOfType<PhysicsComponent>().Velocity.X < 0)
                {
                    enemy.ComponentOfType<PhysicsComponent>().Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * -moveSpeed,
                            enemy.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
                else
                {
                    enemy.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(-moveSpeed,
                        enemy.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
            };
        }

        private static State MoveRightState(GameObject enemy)
        {
            return
                State.Builder()
                    .WithName("Moving Right")
                    .WithAction(MoveRightAction(enemy))
                    .Build();
        }

        private static Action MoveRightAction(GameObject enemy)
        {
            /* TODO: Configify */
            var moveSpeed = 5;
            return gameTime =>
            {
                if (enemy.ComponentOfType<PhysicsComponent>().Velocity.X < 0)
                {
                    enemy.ComponentOfType<PhysicsComponent>().Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * moveSpeed,
                            enemy.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
                else
                {
                    enemy.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(moveSpeed,
                        enemy.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
            };
        }
    }
}
