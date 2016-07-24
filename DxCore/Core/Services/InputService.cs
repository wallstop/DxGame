using DxCore.Core.Input;
using DxCore.Core.Primitives;

namespace DxCore.Core.Services
{
    public class InputService : Service
    {
        public InputHandler InputHandler { get; private set; }

        public override void Initialize()
        {
            InputHandler = new InputHandler();
        }

        public void HighSpeedUpdate(DxGameTime gameTime)
        {
            // TODO
        }

        protected override void Update(DxGameTime gameTime)
        {
            InputHandler.Process(gameTime);
        }
    }
}