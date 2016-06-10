using System.Collections.Generic;
using DxCore.Core.Input;
using DxCore.Core.Primitives;

namespace DxCore.Core.Models
{
    public class InputModel : Model
    {
        public IEnumerable<KeyboardEvent> Events { get; private set; }
        public IEnumerable<KeyboardEvent> FinishedEvents { get; private set; }
        private InputHandler InputHandler { get; set; }

        public override bool ShouldSerialize => false;

        public override void Initialize()
        {
            InputHandler = new InputHandler();
        }

        protected override void Update(DxGameTime gameTime)
        {
            InputHandler.Process(gameTime);
            Events = InputHandler.CurrentEvents;
            FinishedEvents = InputHandler.FinishedEvents;
        }
    }
}