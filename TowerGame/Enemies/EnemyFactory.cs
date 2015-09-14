using System;
using System.Linq;
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
    [Serializable]
    public static class EnemyFactory
    {
        private static readonly float INITIAL_MOVEMENT_BOOST = 1.1f;

        public static StateMachine SimpleBoxBehavior(DxGame game,
            AnimationComponent.AnimationComponentBuilder animationBuilder, GameObject enemy)
        {
            var enemyName = "Box";
            var animationName = "Simple";
            var stateMachineBuilder = StateMachine.Builder();
            stateMachineBuilder.WithDxGame(game);

            var simpleBoxActionResolve = new SimpleBoxActionResolver(enemy);
            var idleState = simpleBoxActionResolve.IdleState;
            // Construct states and use them to build animations

            animationBuilder.WithStateAndAsset(idleState, AnimationFactory.AnimationFor(enemyName, animationName));
            var stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();

            var moveLeftState = MoveLeftState(simpleBoxActionResolve);
            animationBuilder.WithStateAndAsset(moveLeftState, AnimationFactory.AnimationFor(enemyName, animationName));
            var moveRightState = MoveRightState(simpleBoxActionResolve);
            animationBuilder.WithStateAndAsset(moveRightState, AnimationFactory.AnimationFor(enemyName, animationName));

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
                return
                    moveRequests.All(
                        request => request.Direction != Direction.West && request.Direction != Direction.East);
            };

            var returnToIdleTransition = new Transition(returnToIdleTrigger, idleState, Priority.LOW);
            idleState.Transitions.Add(returnToIdleTransition);
            moveLeftState.Transitions.Add(returnToIdleTransition);
            moveRightState.Transitions.Add(returnToIdleTransition);

            return stateMachine;
        }

        private static State MoveLeftState(SimpleBoxActionResolver resolver)
        {
            return
                State.Builder()
                    .WithName("Moving Left")
                    .WithAction(resolver.MoveLeftAction)
                    .Build();
        }

        private static State MoveRightState(SimpleBoxActionResolver resolver)
        {
            return
                State.Builder()
                    .WithName("Moving Right")
                    .WithAction(resolver.MoveRightAction)
                    .Build();
        }

        /* 
            We need to encapsulate all functions that reference an object as members of a serializable class.
            Otherwise, we cannot serialize Enemies over the network.
        */
        [Serializable]
        private class SimpleBoxActionResolver
        {
            private readonly GameObject enemy_;
            public State IdleState { get; private set; }

            public SimpleBoxActionResolver(GameObject enemy)
            {
                enemy_ = enemy;

                IdleState = State.Builder().WithName("Idle").WithAction(IdleAction).Build();
            }

            private void IdleAction(DxGameTime gameTime)
            {
                var existingVelocity = enemy_.ComponentOfType<PhysicsComponent>().Velocity;
                enemy_.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(0, existingVelocity.Y);
            }

            public void MoveLeftAction(DxGameTime gameTime)
            {
                if (enemy_.ComponentOfType<PhysicsComponent>().Velocity.X < 0)
                {
                    enemy_.ComponentOfType<PhysicsComponent>().Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * -5,
                            enemy_.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
                else
                {
                    enemy_.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(-5,
                        enemy_.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
            }

            public void MoveRightAction(DxGameTime gameTime)
            {
                if (enemy_.ComponentOfType<PhysicsComponent>().Velocity.X < 0)
                {
                    enemy_.ComponentOfType<PhysicsComponent>().Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * 5,
                            enemy_.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
                else
                {
                    enemy_.ComponentOfType<PhysicsComponent>().Velocity = new DxVector2(5,
                        enemy_.ComponentOfType<PhysicsComponent>().Velocity.Y);
                }
            }
        }
    }
}