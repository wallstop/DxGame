using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DXGame.Core
{

    public class GameComponentCollection : GameComponent
    {
        public IEnumerable<GameComponent> Components { get; set; }

        public GameComponentCollection(Game game) : base(game)
        {
        }
    }
}
