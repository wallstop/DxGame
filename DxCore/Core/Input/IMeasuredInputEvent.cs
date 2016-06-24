using System;

namespace DxCore.Core.Input
{
    public interface IMeasuredInputEvent
    {
        TimeSpan Duration { get; }
        TimeSpan StartTime { get; }
        bool HeldDown { get; }
        // TODO: Implement
        [Obsolete("Currently not implemented", true)]
        int RepeatCount { get; }
    }
}
