using System;

namespace DxCore.Extension
{
    public static class TimeSpanExtensions
    {
        // Should probably move somewhere else, this is an extension for ease of finding
        public static TimeSpan FromAccurateMilliseconds(this TimeSpan dontCare, double milliseconds)
        {
            long ticks = (long) Math.Round(milliseconds * TimeSpan.TicksPerMillisecond);
            return new TimeSpan(ticks);
        }
    }
}