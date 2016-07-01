using DxCore.Core.Input;
using DxCore.Core.Primitives;

namespace DxCore.Core.Models
{
    public class InputModel : Model
    {
        public InputHandler InputHandler { get; private set; }

        public override void Initialize()
        {
            InputHandler = new InputHandler();
        }

        protected override void Update(DxGameTime gameTime)
        {
            InputHandler.Process(gameTime);
        }
    }
}