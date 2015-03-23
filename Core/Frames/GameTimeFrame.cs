using System;
using System.Collections.Generic;

namespace DXGame.Core.Frames
{
    /*
        TODO: Flesh this out and make FULLY IMMUTABLE
    */
    public class GameTimeFrame
    {
        // TODO: Make some way to finalize this (make it read-only)
        public Dictionary<UniqueId, GameObject> FrameObjects { get; set; }

        public TimeSpan TimeStamp { get; set; }

        // TODO: Find a way to store input on the frame (Player -> Input mapping)
    }
}