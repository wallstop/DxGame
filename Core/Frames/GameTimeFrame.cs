using System;
using System.Collections.Generic;

namespace DXGame.Core.Frames
{
    public class GameTimeFrame : AbstractFrame
    {
        // TODO: Make some way to finalize this (make it read-only)
        public Dictionary<UniqueId, GameObject> FrameObjects { get; set; }

        public TimeSpan TimeStamp { get; protected set; }

        // TODO: REMOVE, THIS IS ONLY FOR TESTING
        public string TestString { get; set; }

        // TODO: Find a way to store input on the frame (Player -> Input mapping)
    }
}