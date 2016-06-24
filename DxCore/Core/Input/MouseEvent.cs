using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Input
{
    [Serializable]
    [DataContract]
    public class MouseEvent : IInputEvent<MouseButton>
    {
        private static readonly TimeSpan HeldDownThreshold = TimeSpan.FromSeconds(1.0f / 2.0f);

        public TimeSpan Duration { get; }
        public TimeSpan StartTime { get; }
        // TODO: Abstract class?
        public bool HeldDown => Duration > HeldDownThreshold;
        public int RepeatCount { get; }
        public MouseButton Source { get; }

        public MouseEvent(MouseButton source, TimeSpan start, TimeSpan duration, int repeatCount = 1)
        {
            Source = source;
            StartTime = start;
            Duration = duration;
            RepeatCount = repeatCount;
        }
    }
}
