using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    /*
        Short and sweet list of things that are different from one timestamp to the next
    */

    public class GameStateDiff : NetworkMessage
    {
        public GameTime GameTime { get; set; }
        public List<GameComponent> Added { get; set; }
        public List<GameComponent> Updated { get; set; }
        public List<GameComponent> Removed { get; set; }
    }
}