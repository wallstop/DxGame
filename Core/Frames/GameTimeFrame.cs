using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Core.Frames
{
    public class GameTimeFrame : AbstractFrame
    {
        // TODO: Make some way to finalize this (make it read-only)
        public Dictionary<UniqueId, GameObject> FrameObjects { get; set; } 

        // TODO: Find a way to store input on the frame (Player -> Input mapping)
    }
}
