using System.Collections.Generic;
using DXGame.Core.Utils;
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

        protected GameComponentCollection(DxGame game) : base(game)
        {
        }

        public void Add()
        {
            Game.Components.Add(this);
            foreach (GameComponent component in Components)
            {
                Validate.IsNotNullOrDefault(component, $"Cannot add a null/default component to {GetType()}");
                Game.Components.Add(component);
            }
        }

        public void Remove()
        {
            Game.Components.Remove(this);
            foreach (GameComponent component in Components)
            {
                Validate.IsNotNullOrDefault(component, $"Cannot remove a null/default component from {GetType()}");
                Game.Components.Remove(component);
            }
        }
    }
}