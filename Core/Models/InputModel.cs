using System.Collections.Generic;
using DXGame.Core.Input;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public class InputModel : Model
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

        protected override void Update(DxGameTime gameTime)
        {
            InputHandler.Process(gameTime);
            Events = InputHandler.CurrentEvents;
            FinishedEvents = InputHandler.FinishedEvents;
        }
    }
}