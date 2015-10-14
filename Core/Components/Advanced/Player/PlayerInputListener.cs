using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Player
{
    /**

        <summary>
            Ideal use is to check input events & fire command events based on gameTime
        </summary>
    */
    delegate void ActionCheck(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime);

    [Serializable]
    [DataContract]
    public class PlayerInputListener : Component
    {
        // TODO: Configify (this should be in player properties or some shit
        private static readonly TimeSpan DROP_THROUGH_PLATFORM_DELAY = TimeSpan.FromSeconds(1);
        [DataMember] private TimeSpan lastDroppedThroughPlatform_ = TimeSpan.FromSeconds(0);

        private List<ActionCheck> ActionChecks
        {
            get
            {
                var actionChecks = new List<ActionCheck>
                {
                    CheckForMoveLeft,
                    CheckForMoveRight,
                    CheckForMoveUp,
                    CheckForMoveDown,
                    CheckForAbility1,
                    CheckForAbility2,
                    CheckForAbility3,
                    CheckForAbility4,
                    CheckForMovement,
                    CheckForInteraction,
                    CheckForAttack
                };
                return actionChecks;
            }
        }

        public PlayerInputListener(DxGame game)
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* TODO: Change state transitions to simply be event handlers based off the events emitted from here */
            var inputModel = DxGame.Model<InputModel>();
            var inputEvents = inputModel.Events.ToList();
            foreach (var actionCheck in ActionChecks)
            {
                actionCheck.Invoke(inputEvents, gameTime);
            }

            base.Update(gameTime);
        }

        private void CheckForMoveLeft(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Left))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveLeft});
            }
        }

        private void CheckForMoveRight(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Right))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveRight});
            }
        }

        private void CheckForMoveUp(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Jump))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveUp});
            }
        }

        private void CheckForMoveDown(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(keyEvent => keyEvent.Key == DxGame.Controls.Down && keyEvent.HeldDown) &&
                (lastDroppedThroughPlatform_ + DROP_THROUGH_PLATFORM_DELAY) < gameTime.TotalGameTime)
            {
                Parent?.BroadcastMessage(new DropThroughPlatformRequest());
                lastDroppedThroughPlatform_ = gameTime.TotalGameTime;
            }
        }

        /* TODO: Need to feed cooldowns into this logic (more paremeters, drawn from active player / player model ?) */

        private void CheckForAttack(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Attack))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Attack});
            }
        }

        private void CheckForAbility1(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Ability1))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability1});
            }
        }

        private void CheckForAbility2(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Ability2))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability2});
            }
        }

        private void CheckForAbility3(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Ability3))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability3});
            }
        }

        private void CheckForAbility4(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Ability4))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability4});
            }
        }

        private void CheckForMovement(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Movement))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Movement});
            }
        }

        private void CheckForInteraction(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Interact))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.InteractWithEnvironment});
            }
        }
    }
}