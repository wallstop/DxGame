using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework.Input;

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
            InputModel inputModel = DxGame.Instance.Model<InputModel>();
            if (inputModel?.FinishedEvents?.Any(inputEvent => inputEvent.Key == DEV_KEY) ?? false)
            {
                DeveloperMode = DeveloperMode.Rotate();
            }
        }
    }
}