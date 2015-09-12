using System.Linq;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.State;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.TowerGame.Behaviors
{
    public static class PlayerBehaviorFactory
    {
        private static readonly float INITIAL_MOVEMENT_BOOST = 1.1f;

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

            var idleState = IdleState(player);
            // TODO: Figure out how to make this generic / modular
            animation.WithStateAndAsset(idleState, AnimationFactory.AnimationFor(playerName, StandardAnimationType.Idle));
            var stateMachine = stateMachineBuilder.WithInitialState(idleState).Build();

            var moveLeftState = MoveLeftState(player);
            animation.WithStateAndAsset(moveLeftState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.WalkingLeft));
            var moveRightState = MoveRightState(player);
            animation.WithStateAndAsset(moveRightState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.WalkingRight));
            var initialJumpState = BeginningJumpState(player);
            animation.WithStateAndAsset(initialJumpState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpLeft));
            var jumpingLeftState = JumpingLeftState(player);
            animation.WithStateAndAsset(jumpingLeftState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpLeft));
            var jumpingRightState = JumpingRightState(player);
            animation.WithStateAndAsset(jumpingRightState,
                AnimationFactory.AnimationFor(playerName, StandardAnimationType.JumpRight));
            var jumpingState = JumpState(player);
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

            Trigger belowCollisionTrigger = (dxGame, gameTime) =>
            {
                if (stateMachine.Parent == null)
                {
                    return false;
                }

                var collisions = stateMachine.Parent.CurrentMessages.OfType<CollisionMessage>();
                return collisions.Any(collision => collision.CollisionDirections.Contains(Direction.South));
            };
            var jumpToIdleTransition = new Transition(belowCollisionTrigger, idleState, Priority.HIGH);

            jumpingLeftState.Transitions.Add(jumpToIdleTransition);
            jumpingRightState.Transitions.Add(jumpToIdleTransition);
            jumpingState.Transitions.Add(jumpToIdleTransition);

            return stateMachine;
        }

        private static State IdleState(Core.Player player)
        {
            return State.Builder()
                .WithName("Idle")
                .WithAction(gameTime => player.Physics.Velocity = new DxVector2(0, player.Physics.Velocity.Y))
                .Build();
        }

        private static State MoveLeftState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Moving Left")
                    .WithAction(MoveLeftAction(player))
                    .Build();
        }

        private static Action MoveLeftAction(Core.Player player)
        {
            return gameTime =>
            {
                if (player.Physics.Velocity.X < 0)
                {
                    player.Physics.Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * -player.Properties.MoveSpeed.CurrentValue,
                            player.Physics.Velocity.Y);
                }
                else
                {
                    player.Physics.Velocity = new DxVector2(-player.Properties.MoveSpeed.CurrentValue,
                        player.Physics.Velocity.Y);
                }
            };
        }

        private static State MoveRightState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Moving Right")
                    .WithAction(MoveRightAction(player))
                    .Build();
        }

        private static State BeginningJumpState(Core.Player player)
        {
            return State.Builder().WithName("Beginning to Jump").WithAction(gameTime =>
            {
                player.Physics.Velocity = new DxVector2(player.Physics.Velocity.X, 0);
                player.Physics.Acceleration = new DxVector2(player.Physics.Acceleration.X,
                    -player.Properties.JumpSpeed.CurrentValue);
            }).Build();
        }

        private static State JumpState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(gameTime => { })
                    .Build();
        }

        private static Action MoveRightAction(Core.Player player)
        {
            return gameTime =>
            {
                if (player.Physics.Velocity.X < 0)
                {
                    player.Physics.Velocity =
                        new DxVector2(INITIAL_MOVEMENT_BOOST * player.Properties.MoveSpeed.CurrentValue,
                            player.Physics.Velocity.Y);
                }
                else
                {
                    player.Physics.Velocity = new DxVector2(player.Properties.MoveSpeed.CurrentValue,
                        player.Physics.Velocity.Y);
                }
            };
        }

        private static State JumpingLeftState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Jumping Left")
                    .WithAction(MoveLeftAction(player))
                    .Build();
        }

        private static State JumpingRightState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Jumping Right")
                    .WithAction(MoveRightAction(player))
                    .Build();
        }

        public static StateMachine GevurahBehavior(DxGame game,
            AnimationComponent.AnimationComponentBuilder animationBuilder, Core.Player player)
        {
            return BasicPlayerStateBuilder(game, animationBuilder, player);
        }
    }
}