using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Input;
using Component = DxCore.Core.Components.Basic.Component;

namespace DxCore.Core.Input
{
    /*
        TODO: Change this to be it's own thread with a high update read, pushing events to a shared, threadsafe queue that is read.
        In other words, redesign. This is short and sweet (and maybe "good enough")
    */

    [Serializable]
    [DataContract]
    public class InputHandler : Component
    {
        private static ButtonState StateFromMouse(ref MouseState mouseState, MouseButton mouseButton)
        {
            switch(mouseButton)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton;
                case MouseButton.Middle:
                    return mouseState.MiddleButton;
                case MouseButton.Right:
                    return mouseState.RightButton;
                case MouseButton.Mouse4:
                    return mouseState.XButton1;
                case MouseButton.Mouse5:
                    return mouseState.XButton2;
                default:
                    throw new InvalidEnumArgumentException($"Unknown {typeof(MouseButton)} {mouseButton}");
            }
        }

        // TODO: PLease, please, come up with a better design for input events (keyboard / gamepad / mouse agnostic) 
        [DataMember]
        public ReadOnlyCollection<KeyboardEvent> CurrentKeyboardEvents { get; private set; }

        [DataMember]
        public ReadOnlyCollection<KeyboardEvent> FinishedKeyboardEvents { get; private set; }

        [DataMember] private MouseState currentMouseState_;

        [DataMember]
        public ReadOnlyCollection<MouseEvent> CurrentMouseEvents { get; private set; }

        [DataMember]
        public ReadOnlyCollection<MouseEvent> FinishedMouseEvents { get; private set; }

        public InputHandler()
        {
            CurrentKeyboardEvents = new ReadOnlyCollection<KeyboardEvent>(new List<KeyboardEvent>());
            FinishedKeyboardEvents = new ReadOnlyCollection<KeyboardEvent>(new List<KeyboardEvent>());

            CurrentMouseEvents = new ReadOnlyCollection<MouseEvent>(new List<MouseEvent>());
            FinishedMouseEvents = new ReadOnlyCollection<MouseEvent>(new List<MouseEvent>());
            currentMouseState_ = Mouse.GetState();
            UpdatePriority = UpdatePriority.Input;
        }

        protected override void Update(DxGameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            List<Keys> currentKeys = keyboardState.GetPressedKeys().ToList();
            DetermineKeyboardEvents(gameTime, currentKeys);

            MouseState mouseState = Mouse.GetState();
            DetermineMouseEvents(gameTime, mouseState);
        }

        private void DetermineMouseEvents(DxGameTime gameTime, MouseState newMouseState)
        {
            TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            TimeSpan currentTime = gameTime.TotalGameTime;
            
            List<MouseEvent> newlyFinishedEvents = new List<MouseEvent>();

            List<MouseButton> uncheckedButtons =
                new List<MouseButton>((MouseButton[]) Enum.GetValues(typeof(MouseButton)));
            List<MouseEvent> newEvents = new List<MouseEvent>();

            foreach(MouseEvent mouseEvent in CurrentMouseEvents)
            {
                if(StateFromMouse(ref newMouseState, mouseEvent.Source) == ButtonState.Pressed)
                {
                    MouseEvent newMouseEvent = new MouseEvent(mouseEvent.Source, mouseEvent.StartTime,
                        mouseEvent.Duration + elapsedTime);
                    newEvents.Add(newMouseEvent);
                    uncheckedButtons.Remove(mouseEvent.Source);
                }
                else
                {
                    // TODO: Correct-ify?
                    MouseEvent finishedEvent = new MouseEvent(mouseEvent.Source, currentTime, TimeSpan.Zero);
                    newlyFinishedEvents.Add(finishedEvent);
                }
            }

            foreach(MouseButton mouseButton in uncheckedButtons)
            {
                ButtonState newState = StateFromMouse(ref newMouseState, mouseButton);
                if(newState == ButtonState.Pressed)
                {
                    MouseEvent newMouseEvent = new MouseEvent(mouseButton, currentTime, TimeSpan.Zero);
                    newEvents.Add(newMouseEvent);
                }
            }

            CurrentMouseEvents = new ReadOnlyCollection<MouseEvent>(newEvents);
            FinishedMouseEvents = new ReadOnlyCollection<MouseEvent>(newlyFinishedEvents);
        }

        private void DetermineKeyboardEvents(DxGameTime gameTime, List<Keys> currentKeys)
        {
            TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            TimeSpan currentTime = gameTime.TotalGameTime;

            List<KeyboardEvent> newEvents = new List<KeyboardEvent>();
            List<KeyboardEvent> finishedEvents = new List<KeyboardEvent>();

            // Update last frame's current events
            foreach(KeyboardEvent keyEvent in CurrentKeyboardEvents)
            {
                Keys key = keyEvent.Source;
                if(currentKeys.Contains(key))
                {
                    // TODO: Copy over to new events, make immutable
                    keyEvent.Duration += elapsedTime;
                    newEvents.Add(keyEvent);
                    currentKeys.Remove(key);
                }
                else
                {
                    finishedEvents.Add(new KeyboardEvent
                    {
                        Source = key,
                        StartTime = currentTime,
                        /*  
                            TODO: Fix, this isn't... correct, depending on your view point.
                            Alternatively, document 
                        */
                        Duration = TimeSpan.FromSeconds(0)
                    });
                }
            }

            // Check for new events
            foreach(KeyboardEvent keyEvent in
                currentKeys.Select(
                    key => new KeyboardEvent {Source = key, Duration = TimeSpan.Zero, StartTime = currentTime})
                )
            {
                newEvents.Add(keyEvent);
            }

            CurrentKeyboardEvents = new ReadOnlyCollection<KeyboardEvent>(newEvents);
            FinishedKeyboardEvents = new ReadOnlyCollection<KeyboardEvent>(finishedEvents);
        }
    }
}