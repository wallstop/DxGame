using System;
using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    /*
        Short and sweet list of things that are different from one timestamp to the next
    */

    public class GameStateDiff : NetworkMessage
    {
        public GameTime GameTime = new GameTime();
        public List<IGameComponent> Added = new List<IGameComponent>();
        public List<IGameComponent> Updated = new List<IGameComponent>();
        public List<IGameComponent> Removed = new List<IGameComponent>();
    }
}