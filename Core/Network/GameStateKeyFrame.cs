using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    /*
        Short and sweet All-Data dump of a client's gamestate
    */

    public class GameStateKeyFrame : NetworkMessage
    {
        public GameTime GameTime { get; set; }
        public List<GameComponent> Components { get; set; }
    }
}