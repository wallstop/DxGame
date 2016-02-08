using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Command
{
    /**

        <summary>
            Given a set of input events & the current gameTime (TODO: Remove gameTime & delegate cooldown checks appropriately),
            fires a CommandMessage if (specific condition) is met.
        </summary>
    */

    internal delegate void ActionCheck(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime);

    /**

        <summary>
            Checks input events & fires commandments for any and all events. 
            Properly syncs up with current controls.
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class PlayerInputListener : AbstractCommandComponent
    {
        // TODO: Configify (this should be in player properties or some shit)
        private static readonly TimeSpan DROP_THROUGH_PLATFORM_DELAY = TimeSpan.FromSeconds(1);

        [IgnoreDataMember] private List<ActionCheck> cachedActionChecks_;

        [DataMember] [ProtoMember(1)] private TimeSpan lastDroppedThroughPlatform_ = TimeSpan.FromSeconds(0);

        private IEnumerable<ActionCheck> ActionChecks
        {
            get
            {
                // This isn't threadsafe
                if (cachedActionChecks_ != null)
                {
                    return cachedActionChecks_;
                }

                /* Rip every function that is a ActionCheck off of the class via Reflection (what could go wrong?) */
                var allMethods = typeof (PlayerInputListener).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var actionChecks = new List<ActionCheck>(allMethods.Length);
                actionChecks.AddRange(
                    allMethods.Select(
                        method => (ActionCheck) Delegate.CreateDelegate(typeof (ActionCheck), this, method, false))
                        .Where(actionCheck => !ReferenceEquals(null, actionCheck)));
                /* Cache them so this is only a one-time hit */
                cachedActionChecks_ = actionChecks;
                return cachedActionChecks_;
            }
        }

        public PlayerInputListener()
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* TODO: Change state transitions to simply be event handlers based off the events emitted from here */
            var inputModel = DxGame.Instance.Model<InputModel>();
            var inputEvents = inputModel.Events.ToList();
            foreach (var actionCheck in ActionChecks)
            {
                actionCheck.Invoke(inputEvents, gameTime);
            }

            base.Update(gameTime);
        }

        private void CheckForMoveLeft(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Left))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveLeft});
            }
        }

        private void CheckForMoveRight(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Right))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveRight});
            }
        }

        private void CheckForMoveUp(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Jump))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveUp});
            }
        }

        private void CheckForMoveDown(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(keyEvent => keyEvent.Key == DxGame.Instance.Controls.Down && keyEvent.HeldDown) &&
                (lastDroppedThroughPlatform_ + DROP_THROUGH_PLATFORM_DELAY) < gameTime.TotalGameTime)
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.MoveDown});
                Parent?.BroadcastMessage(new DropThroughPlatformRequest()); // TODO: Move out to somewhere else?
                lastDroppedThroughPlatform_ = gameTime.TotalGameTime;
            }
        }

        /* TODO: Need to feed cooldowns into this logic (more paremeters, drawn from active player / player model ?) */

        private void CheckForAttack(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Attack))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Attack});
            }
        }

        private void CheckForAbility1(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Ability1))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability1});
            }
        }

        private void CheckForAbility2(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Ability2))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability2});
            }
        }

        private void CheckForAbility3(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Ability3))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability3});
            }
        }

        private void CheckForAbility4(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Ability4))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Ability4});
            }
        }

        private void CheckForMovement(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Movement))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.Movement});
            }
        }

        private void CheckForInteraction(List<Input.KeyboardEvent> inputEvents, DxGameTime gameTime)
        {
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Instance.Controls.Interact))
            {
                Parent?.BroadcastMessage(new CommandMessage {Commandment = Commandment.InteractWithEnvironment});
                DxGame.Instance.BroadcastMessage(new EnvironmentInteractionMessage {Source = Parent, Time = gameTime}); // TODO: Move somewhere else?
            }
        }
    }
}