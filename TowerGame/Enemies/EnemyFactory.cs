using System;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Action = DXGame.Core.State.Action;

namespace DXGame.TowerGame.Enemies
{
    [Serializable]
    public static class EnemyFactory
    {

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
                var decelerationForce = "deceleration";
                var movement = new Movement(decelerationForce);
                enemy_.ComponentOfType<PhysicsComponent>().AttachForce(movement.Force);
            }

            public void MoveLeftAction(DxGameTime gameTime)
            {
                var moveLeftForce = "moveLeftForce";
                var moveLeftVelocityVector = new DxVector2(-5, 0);
                var movement = new Movement(moveLeftVelocityVector, moveLeftForce);
                enemy_.ComponentOfType<PhysicsComponent>().AttachForce(movement.Force);
            }

            public void MoveRightAction(DxGameTime gameTime)
            {
                var moveRightForce = "moveRightForce";
                var moveRightVelocityVector = new DxVector2(5, 0);
                var movement = new Movement(moveRightVelocityVector, moveRightForce);
                enemy_.ComponentOfType<PhysicsComponent>().AttachForce(movement.Force);
            }
        }

        [Serializable]
        private class Movement
        {
            public Force Force { get; }
            private bool dissipated_ = false;
            private DxVector2 Direction { get; }

            public Movement( DxVector2 directionalForceVector, string forceName)
            {
                Direction = directionalForceVector;
                Force = new Force(new DxVector2(), directionalForceVector, DissipationFunction, forceName);
            }

            public Movement(string forceName)
            {
                Direction = new DxVector2();
                Force = new Force(new DxVector2(), new DxVector2(), DecelerationFunction, forceName);
            }

            private Tuple<bool, DxVector2> DecelerationFunction(DxVector2 externalVelocity,
                DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
            {
                var hasDissipated = dissipated_;
                dissipated_ = true;
                /* Special expire-after-one-frame deceleration force */
                return Tuple.Create(hasDissipated,
                    WorldForces.HorizontalVelocityDissipation(externalVelocity, externalAcceleration,
                        currentAcceleration, gameTime).Item2);
            }

            private Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity,
                DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
            {
                var xDirectionSign = Math.Sign(Direction.X);
                var xIsNegative = xDirectionSign == -1;
                DxVector2 accelerationVector = new DxVector2 {X = xIsNegative ? Math.Min(Direction.X - externalVelocity.X, 0) : Math.Max(Direction.X - externalVelocity.X, 0)};
                accelerationVector.X = xIsNegative ? Math.Max(accelerationVector.X, Direction.X) : Math.Min(accelerationVector.X, Direction.X);

                var yDirectionSign = Math.Sign(Direction.Y);
                var yIsNegative = yDirectionSign == -1;

                accelerationVector.Y = yIsNegative ? Math.Min(Direction.Y - externalVelocity.Y, 0) : Math.Max(Direction.Y - externalVelocity.Y, 0);
                accelerationVector.Y = yIsNegative ? Math.Max(accelerationVector.Y, Direction.Y) : Math.Min(accelerationVector.Y, Direction.Y);
                var hasDissipated = dissipated_; // Dissipate after one frame always (we need continual move left requests to actually move)
                dissipated_ = true;
                return Tuple.Create(hasDissipated, accelerationVector);
            }
        }
    }
}