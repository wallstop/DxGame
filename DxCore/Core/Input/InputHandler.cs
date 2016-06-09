using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Input;

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
        [DataMember]
        public List<KeyboardEvent> CurrentEvents { get; private set; }
        [DataMember]
        public List<KeyboardEvent> FinishedEvents { get; private set; }

        public InputHandler()
        {
            CurrentEvents = new List<KeyboardEvent>();
            FinishedEvents = new List<KeyboardEvent>();
            UpdatePriority = UpdatePriority.INPUT;
        }

        protected override void Update(DxGameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            List<Keys> currentKeys = keyboardState.GetPressedKeys().ToList();

            DetermineKeyboardEvents(gameTime, currentKeys);
        }

        private void DetermineKeyboardEvents(DxGameTime gameTime, List<Keys> currentKeys)
        {
            var elapsedTime = gameTime.ElapsedGameTime;
            var currentTime = gameTime.TotalGameTime;

            FinishedEvents.Clear();

            // Update last frame's current events
            foreach (KeyboardEvent keyEvent in CurrentEvents)
            {
                Keys key = keyEvent.Key;
                if (currentKeys.Contains(key))
                {
                    keyEvent.Duration += elapsedTime;
                    currentKeys.Remove(key);
                }
                else
                {
                    FinishedEvents.Add(new KeyboardEvent
                    {
                        Key = key,
                        StartTime = currentTime,
                        Duration = TimeSpan.FromSeconds(0)
                    });
                }
            }

            // Check for new events
            foreach (KeyboardEvent keyEvent in currentKeys.Select(key => new KeyboardEvent
            {
                Key = key,
                Duration = TimeSpan.FromSeconds(0),
                StartTime = currentTime
            }))
            {
                CurrentEvents.Add(keyEvent);
            }

            // Remove all the CurrentEvents where an event has the same key as one in FinishedEvents
            CurrentEvents.RemoveAll(key => FinishedEvents.Select(finishedKey => finishedKey.Key).Contains(key.Key));
        }
    }
}