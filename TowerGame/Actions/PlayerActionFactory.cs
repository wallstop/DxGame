using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.TowerGame.Actions
{
    [Serializable]
    public static class PlayerActionFactory
    {
        private static readonly float INITIAL_MOVEMENT_BOOST = 1.1f;

        private static readonly DxVector2 INITIAL_JUMP_ACCELERATION = new DxVector2(0, -1.98f);

        private static readonly DissipationFunction INITIAL_JUMP_DISSIPATION =
            (externalVelocity, externalAcceleration, acceleration, gameTime) =>
            {
                /* If our jumping force is applying a (down into the ground) acceleration, we're done! */
                var done = acceleration.Y > 0 || externalVelocity.Y > 0;
                if (done)
                {
                    return Tuple.Create(true, new DxVector2());
                }
                var scale = gameTime.DetermineScaleFactor(DxGame.Instance);
                acceleration += (WorldForces.GRAVITY_ACCELERATION * scale);
                return Tuple.Create(false, acceleration);
            };

        public static StateMachine BasicPlayerBehavior(DxGame game)
        {
            return null;
        }

        private static StateMachine BasicPlayerStateBuilder(DxGame game,
            AnimationComponent.AnimationComponentBuilder animation, Core.Player player)
        {
            /* 
                WTF StateMachine creation BLOWWWWWWWWWS. Figure out some way to automate this / 
                make wiring things up SIGNFICANTLY less painful 

                TODO: Clean this the hell up
            */
            var playerName = "Player";
            var stateMachineBuilder = StateMachine.Builder();
            stateMachineBuilder.WithDxGame(game);

            var simplePlayerActionResolver = new SimplePlayerActionResolver(player);

            var idleState = IdleState(simplePlayerActionResolver);
            // TODO: Figure out how to make this generic / modular
            animation.WithStateAndAsset(idleState, AnimationFactory.AnimationFor(playerName, StandardAnimationType.Idle));
            var stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();
            simplePlayerActionResolver.StateMachine = stateMachine;

            var moveLeftState = MoveLeftState(simplePlayerActionResolver);
            animation.WithStateAndAsset(moveLeftState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.WalkingLeft));
            var moveRightState = MoveRightState(simplePlayerActionResolver);
            animation.WithStateAndAsset(moveRightState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.WalkingRight));
            var initialJumpState = BeginningJumpState(simplePlayerActionResolver);
            animation.WithStateAndAsset(initialJumpState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpLeft));
            var jumpingLeftState = JumpingLeftState(simplePlayerActionResolver);
            animation.WithStateAndAsset(jumpingLeftState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpLeft));
            var jumpingRightState = JumpingRightState(simplePlayerActionResolver);
            animation.WithStateAndAsset(jumpingRightState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpRight));
            var jumpingState = JumpState(simplePlayerActionResolver);
            animation.WithStateAndAsset(jumpingState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpLeft));

            Trigger moveLeftTrigger =
                (playerInstance, gameTime) =>
                    DxGame.Instance.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Left);
            var moveLeftTransition = new Transition(moveLeftTrigger, moveLeftState);
            var jumpLeftTransition = new Transition(moveLeftTrigger, jumpingLeftState);

            idleState.Transitions.Add(moveLeftTransition);
            moveLeftState.Transitions.Add(moveLeftTransition);
            moveRightState.Transitions.Add(moveLeftTransition);
            jumpingState.Transitions.Add(jumpLeftTransition);
            jumpingLeftState.Transitions.Add(jumpLeftTransition);
            jumpingRightState.Transitions.Add(jumpLeftTransition);

            Trigger moveRightTrigger =
                (playerInstance, gameTime) =>
                    DxGame.Instance.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Right);
            var moveRightTransition = new Transition(moveRightTrigger, moveRightState);
            var jumpRightTransition = new Transition(moveRightTrigger, jumpingRightState);

            idleState.Transitions.Add(moveRightTransition);
            moveLeftState.Transitions.Add(moveRightTransition);
            moveRightState.Transitions.Add(moveRightTransition);
            jumpingState.Transitions.Add(jumpRightTransition);
            jumpingLeftState.Transitions.Add(jumpRightTransition);
            jumpingRightState.Transitions.Add(jumpRightTransition);

            Trigger jumpTrigger =
                (playerInstance, gameTime) =>
                    DxGame.Instance.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Jump);
            var jumpTransition = new Transition(jumpTrigger, initialJumpState, Priority.HIGH);
            moveLeftState.Transitions.Add(jumpTransition);
            moveRightState.Transitions.Add(jumpTransition);
            idleState.Transitions.Add(jumpTransition);

            Trigger initialToRealJumpTrigger = (playerInstance, gameTime) => true;
            initialJumpState.Transitions.Add(new Transition(initialToRealJumpTrigger, jumpingState));

            var idleTrigger = new Trigger((playerInstance, gameTime) => !DxGame.Instance.Model<InputModel>().Events.Any());
            var notMovingLeftTrigger =  new Trigger((playerInstance, gameTime) => !(DxGame.Instance.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Left)));
            var notMovingRightTrigger = new Trigger((playerInstance, gameTime) =>
                !(DxGame.Instance.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Right)));
            var idleMoveTransition = new Transition(idleTrigger, idleState, Priority.LOW);
            var idleJumpTransition = new Transition(idleTrigger, jumpingState, Priority.LOW);
            idleState.Transitions.Add(idleMoveTransition);
            moveLeftState.Transitions.Add(idleMoveTransition);
            moveLeftState.Transitions.Add(new Transition(notMovingLeftTrigger, idleState, Priority.LOW));
            moveRightState.Transitions.Add(idleMoveTransition);
            moveRightState.Transitions.Add(new Transition(notMovingRightTrigger, idleState, Priority.LOW));
            jumpingState.Transitions.Add(idleJumpTransition);
            jumpingLeftState.Transitions.Add(idleJumpTransition);
            jumpingRightState.Transitions.Add(idleJumpTransition);

            var jumpToIdleTransition = new Transition(simplePlayerActionResolver.BelowCollisionTrigger, idleState, Priority.HIGH);

            jumpingLeftState.Transitions.Add(jumpToIdleTransition);
            jumpingRightState.Transitions.Add(jumpToIdleTransition);
            jumpingState.Transitions.Add(jumpToIdleTransition);

            return stateMachine;
        }
        
        [Serializable]
        [DataContract]
        private class SimplePlayerActionResolver
        {
            [DataMember]
            private readonly Core.Player player_;

            [DataMember]
            public StateMachine StateMachine { get; set; }

            public SimplePlayerActionResolver(Core.Player player)
            {
                player_ = player;
            }

            public bool BelowCollisionTrigger(GameObject playerInstance, DxGameTime gameTime)
            {
                if (StateMachine.Parent == null)
                {
                    return false;
                }

                var collisions = StateMachine.Parent.CurrentMessages.OfType<CollisionMessage>();
                return collisions.Any(collision => collision.CollisionDirections.Contains(Direction.South));
            }

            public void MoveLeftAction(DxGameTime gameTime)
            {
                if (player_.Physics.Velocity.X < 0)
                {
                    player_.Physics.Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * -player_.Properties.MoveSpeed.CurrentValue,
                            player_.Physics.Velocity.Y);
                }
                else
                {
                    player_.Physics.Velocity = new DxVector2(-player_.Properties.MoveSpeed.CurrentValue,
                        player_.Physics.Velocity.Y);
                }
            }

            public void MoveRightAction(DxGameTime gameTime)
            {
                if (player_.Physics.Velocity.X < 0)
                {
                    player_.Physics.Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * player_.Properties.MoveSpeed.CurrentValue,
                            player_.Physics.Velocity.Y);
                }
                else
                {
                    player_.Physics.Velocity = new DxVector2(player_.Properties.MoveSpeed.CurrentValue,
                        player_.Physics.Velocity.Y);
                }
            }

            public void IdleAction(DxGameTime gameTime)
            {
                player_.Physics.Velocity = new DxVector2(0, player_.Physics.Velocity.Y);
            }

            public void BeginningJumpAction(DxGameTime gameTime)
            {
                player_.Physics.AttachForce(new Force( new DxVector2(0, - player_.Properties.JumpSpeed.CurrentValue), INITIAL_JUMP_ACCELERATION, INITIAL_JUMP_DISSIPATION, "PlayerJump"));
            }

            public void JumpAction(DxGameTime gameTime)
            {
            }
        }

        private static State IdleState(SimplePlayerActionResolver actionResolver)
        {
            return State.Builder()
                .WithName("Idle")
                .WithAction(actionResolver.IdleAction)
                .Build();
        }

        private static State MoveLeftState(SimplePlayerActionResolver actionResolver)
        {
            return
                State.Builder()
                    .WithName("Moving Left")
                    .WithAction(actionResolver.MoveLeftAction)
                    .Build();
        }

        private static State MoveRightState(SimplePlayerActionResolver actionResolver)
        {
            return
                State.Builder()
                    .WithName("Moving Right")
                    .WithAction(actionResolver.MoveRightAction)
                    .Build();
        }

        private static State BeginningJumpState(SimplePlayerActionResolver actionResolver)
        {
            return State.Builder().WithName("Beginning to Jump").WithAction(actionResolver.BeginningJumpAction).Build();
        }

        private static State JumpState(SimplePlayerActionResolver actionResolver)
        {
            return
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(actionResolver.JumpAction)
                    .Build();
        }

        private static State JumpingLeftState(SimplePlayerActionResolver actionResolver)
        {
            return
                State.Builder()
                    .WithName("Jumping Left")
                    .WithAction(actionResolver.MoveLeftAction)
                    .Build();
        }

        private static State JumpingRightState(SimplePlayerActionResolver actionResolver)
        {
            return
                State.Builder()
                    .WithName("Jumping Right")
                    .WithAction(actionResolver.MoveRightAction)
                    .Build();
        }

        public static StateMachine GevurahBehavior(DxGame game,
            AnimationComponent.AnimationComponentBuilder animationBuilder, Core.Player player)
        {
            return BasicPlayerStateBuilder(game, animationBuilder, player);
        }
    }
}