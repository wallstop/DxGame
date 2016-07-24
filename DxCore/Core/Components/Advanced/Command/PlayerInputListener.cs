using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DxCore.Core.Input;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Command
{
    /**

        <summary>
            Given a set of input events & the current gameTime (TODO: Remove gameTime & delegate cooldown checks appropriately),
            fires a CommandMessage if (specific condition) is met.
        </summary>
    */

    internal delegate bool ActionCheck(List<KeyboardEvent> inputEvents, ref Commandment commandment);

    /**

        <summary>
            Checks input events & fires commandments for any and all events. 
            Properly syncs up with current controls.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class PlayerInputListener : AbstractCommandComponent
    {
        [IgnoreDataMember] private static List<ActionCheck> CACHED_ACTION_CHECKS;

        /* We use a list instead of single KeyboardEvents to check the case of "combo" type inputs */

        [DataMember]
        private Func<List<KeyboardEvent>> PlayerInputProducer { get; set; }

        private static IEnumerable<ActionCheck> ActionChecks
        {
            get
            {
                // This isn't threadsafe
                if(CACHED_ACTION_CHECKS != null)
                {
                    return CACHED_ACTION_CHECKS;
                }

                /* Rip every function that is a ActionCheck off of the class via Reflection (what could go wrong?) */
                var allMethods = typeof(PlayerInputListener).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
                var actionChecks = new List<ActionCheck>(allMethods.Length);
                actionChecks.AddRange(
                    allMethods.Select(
                        method => (ActionCheck) Delegate.CreateDelegate(typeof(ActionCheck), method, false))
                        .Where(actionCheck => !ReferenceEquals(null, actionCheck)));
                /* Cache them so this is only a one-time hit */
                CACHED_ACTION_CHECKS = actionChecks;
                return CACHED_ACTION_CHECKS;
            }
        }

        public PlayerInputListener() : this(RipEventsFromLocalInputModel) {}

        public PlayerInputListener(Func<List<KeyboardEvent>> playerInputProducer)
        {
            Validate.Hard.IsNotNullOrDefault(playerInputProducer,
                this.GetFormattedNullOrDefaultMessage(nameof(playerInputProducer)));
            PlayerInputProducer = playerInputProducer;
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* TODO: Change state transitions to simply be event handlers based off the events emitted from here */

            List<KeyboardEvent> inputEvents = PlayerInputProducer.Invoke();
            List<Commandment> commandments = DetermineCommandmentsFor(inputEvents);
            foreach(Commandment commandment in commandments)
            {
                BroadcastCommandment(commandment);
            }
            base.Update(gameTime);
        }

        public static List<KeyboardEvent> RipEventsFromLocalInputModel()
        {
            InputService inputService = DxGame.Instance.Service<InputService>();
            List<KeyboardEvent> inputEvents = inputService.InputHandler.CurrentKeyboardEvents.ToList();
            return inputEvents;
        }

        public static List<Commandment> DetermineCommandmentsFor(List<KeyboardEvent> keyboardEvents)
        {
            List<Commandment> commandments = ActionChecks.Select(actionCheck =>
            {
                Commandment commandment = Commandment.None;
                if(actionCheck.Invoke(keyboardEvents, ref commandment))
                {
                    Commandment? actualCommandment = commandment;
                    return actualCommandment;
                }
                return null;
            }).Where(commandment => commandment.HasValue).Select(commandment => commandment.Value).ToList();
            if(commandments.Any())
            {
                return commandments;
            }
            return new List<Commandment>(1) {Commandment.None};
        }

        private static bool CheckForMoveLeft(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Left))
            {
                commandment = Commandment.MoveLeft;
                return true;
            }
            return false;
        }

        private static bool CheckForMoveRight(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Right))
            {
                commandment = Commandment.MoveRight;
                return true;
            }
            return false;
        }

        private static bool CheckForMoveUp(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Jump))
            {
                commandment = Commandment.MoveUp;
                return true;
            }
            return false;
        }

        private static bool CheckForMoveDown(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(keyEvent => keyEvent.Source == DxGame.Instance.Controls.Down))
            {
                commandment = Commandment.MoveDown;
                return true;
            }
            return false;
        }

        private static bool CheckForAttack(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Attack))
            {
                commandment = Commandment.Attack;
                return true;
            }
            return false;
        }

        private static bool CheckForAbility1(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Ability1))
            {
                commandment = Commandment.Ability1;
                return true;
            }
            return false;
        }

        private static bool CheckForAbility2(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Ability2))
            {
                commandment = Commandment.Ability2;
                return true;
            }
            return false;
        }

        private static bool CheckForAbility3(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Ability3))
            {
                commandment = Commandment.Ability3;
                return true;
            }
            return false;
        }

        private static bool CheckForAbility4(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Ability4))
            {
                commandment = Commandment.Ability4;
                return true;
            }
            return false;
        }

        private static bool CheckForMovement(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Movement))
            {
                commandment = Commandment.Movement;
                return true;
            }
            return false;
        }

        private static bool CheckForInteraction(List<KeyboardEvent> inputEvents, ref Commandment commandment)
        {
            if(inputEvents.Any(inputEvent => inputEvent.Source == DxGame.Instance.Controls.Interact))
            {
                commandment = Commandment.InteractWithEnvironment;
                return true;
            }
            return false;
        }
    }
}