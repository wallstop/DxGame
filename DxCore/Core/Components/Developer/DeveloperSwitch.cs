using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using Microsoft.Xna.Framework.Input;
using WallNetCore.Extension;

namespace DxCore.Core.Components.Developer
{
    public enum DeveloperMode
    {
        FullOn,
        KindaOn,
        NotSoOn
    }

    /**
        <summary> 
            Simple Component that keeps track of the current "DeveloperMode" state 
        </summary>
    */

    public class DeveloperSwitch : Component
    {
        private static readonly Keys DEV_KEY = Keys.F7;
        public DeveloperMode DeveloperMode { get; private set; } = DeveloperMode.NotSoOn;

        protected override void Update(DxGameTime gameTime)
        {
            InputService inputService = DxGame.Instance.Service<InputService>();
            if(inputService?.InputHandler.FinishedKeyboardEvents?.Any(inputEvent => inputEvent.Source == DEV_KEY) ??
               false)
            {
                DeveloperMode = DeveloperMode.Rotate();
            }
        }
    }
}