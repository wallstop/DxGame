using System.Collections.Generic;
using DXGame.Core.Input;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Models
{
    public class InputModel : GameComponentCollection
    {
        public IEnumerable<KeyboardEvent> Events { get; private set; }
        public IEnumerable<KeyboardEvent> FinishedEvents { get; private set; }

        private InputHandler InputHandler { get; set; }

        public InputModel(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            InputHandler = new InputHandler(DxGame);
        }

        public override void Update(GameTime gameTime)
        {
            InputHandler.Update(gameTime);
            Events = InputHandler.CurrentEvents;
            FinishedEvents = InputHandler.FinishedEvents;
        }
    }
}