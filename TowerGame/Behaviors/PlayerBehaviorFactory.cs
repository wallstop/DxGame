using System.Linq;
using DXGame.Core.Behavior;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.TowerGame.Behaviors
{
    public static class PlayerBehaviorFactory
    {
        private static readonly float INITIAL_MOVEMENT_BOOST = 1.5f;

        public static Behavior BasicPlayerBehavior(DxGame game)
        {
            return null;
        }

        private static Behavior.BehaviorBuilder BasicPlayerBehaviorBuilder(DxGame game, Core.Player player)
        {
            var behaviorBuilder = Behavior.Builder();
            behaviorBuilder.WithDxGame(game);

            var idleState = IdleState(player);
            var behavior = behaviorBuilder.WithInitialState(idleState).Build();

            var moveLeftState = MoveLeftState(player);
            var moveRightState = MoveRightState(player);
            var initialJumpState = BeginningJumpState(player);
            var jumpingLeftState = JumpingLeftState(player);
            var jumpingRightState = JumpingRightState(player);
            var jumpingState = JumpState(player);

            Trigger moveLeftTrigger =
                (dxGame, gameTime) =>
                    dxGame.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == dxGame.Controls.Left);
            var moveLeftTransition = new Transition(moveLeftTrigger, moveLeftState);
            var jumpLeftTransition = new Transition(moveLeftTrigger, jumpingLeftState);

            idleState.Transitions.Add(moveLeftTransition);
            moveLeftState.Transitions.Add(moveLeftTransition);
            moveRightState.Transitions.Add(moveLeftTransition);
            jumpingState.Transitions.Add(jumpLeftTransition);
            jumpingLeftState.Transitions.Add(jumpLeftTransition);
            jumpingRightState.Transitions.Add(jumpLeftTransition);

            Trigger moveRightTrigger =
                (dxGame, gameTime) =>
                    dxGame.Model<InputModel>().Events.Any(keyEvent => keyEvent.Key == dxGame.Controls.Right);
            var moveRightTransition = new Transition(moveRightTrigger, moveRightState);
            var jumpRightTransition = new Transition(moveRightTrigger, jumpingRightState);

            idleState.Transitions.Add(moveRightTransition);
            moveLeftState.Transitions.Add(moveRightTransition);
            moveRightState.Transitions.Add(moveRightTransition);
            jumpingState.Transitions.Add(jumpRightTransition);
            jumpingLeftState.Transitions.Add(jumpRightTransition);
            jumpingRightState.Transitions.Add(jumpRightTransition);

            var idleTrigger = new Trigger((dxGame, gameTime) => !dxGame.Model<InputModel>().Events.Any());
            var idleMoveTransition = new Transition(idleTrigger, idleState);
            var idleJumpTransition = new Transition(idleTrigger, jumpingState);
            idleState.Transitions.Add(idleMoveTransition);
            moveLeftState.Transitions.Add(idleMoveTransition);
            moveRightState.Transitions.Add(idleMoveTransition);
            jumpingState.Transitions.Add(idleJumpTransition);
            jumpingLeftState.Transitions.Add(idleJumpTransition);
            jumpingRightState.Transitions.Add(idleJumpTransition);

            // TODO

            behavior.RegisterMessageHandler(typeof (CollisionMessage), message =>
            {
                // TODO
            });
            return null;
        }

        private static State IdleState(Core.Player player)
        {
            return State.Builder()
                .WithName("Idle")
                .WithAction(gameTime => player.Physics.Velocity = new DxVector2(0, player.Physics.Velocity.Y))
                .WithPresentation(player.Animation.Draw).Build();
        }

        private static State MoveLeftState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Moving Left")
                    .WithAction(MoveLeftAction(player))
                    .WithPresentation(player.Animation.Draw)
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
                    .WithPresentation(player.Animation.Draw)
                    .Build();
        }

        private static State BeginningJumpState(Core.Player player)
        {
            return State.Builder().WithName("Beginning to Jump").WithAction(gameTime =>
            {
                player.Physics.Velocity = new DxVector2(player.Physics.Velocity.X, 0);
                player.Physics.Acceleration = new DxVector2(player.Physics.Acceleration.X,
                    -player.Properties.JumpSpeed.CurrentValue);
            }).WithPresentation(player.Animation.Draw).Build();
        }

        private static State JumpState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Jumping")
                    .WithAction(gameTime => { })
                    .WithPresentation(player.Animation.Draw)
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
                    .WithPresentation(player.Animation.Draw)
                    .Build();
        }

        private static State JumpingRightState(Core.Player player)
        {
            return
                State.Builder()
                    .WithName("Jumping Right")
                    .WithAction(MoveRightAction(player))
                    .WithPresentation(player.Animation.Draw)
                    .Build();
        }

        public static Behavior GeruvahBehavior(DxGame game)
        {
            // TODO
            return null;
        }
    }
}