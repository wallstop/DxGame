using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core
{

    public abstract class GameComponentCollection : GameComponent
    {
        public List<GameComponent> Components { get; set; }

        public DxGame DxGame
        {
            get { return (DxGame) Game; }
        }

        public GameComponentCollection(DxGame game) : base(game)
        {
        }

        public void Add()
        {
            Game.Components.Add(this);
            foreach (GameComponent component in Components)
            {
                Game.Components.Add(component);
            }
        }

        public void Remove()
        {
            Game.Components.Remove(this);
            foreach (GameComponent component in Components)
            {
                Game.Components.Remove(component);
            }
        }

        // TODO: Check for already-existing status
        public void AddGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                var list = gameObject.ComponentsOfType<GameComponent>();
                Components.AddRange(gameObject.ComponentsOfType<GameComponent>());
                foreach (GameComponent component in list)
                {
                    Game.Components.Add(component);
                }
            }
        }
    }
}
