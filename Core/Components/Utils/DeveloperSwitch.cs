using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Utils
{
    public enum DeveloperMode
    {
        FullOn,
        NotSoOn
    }

    public class DeveloperSwitch : Component
    {
        private static readonly Keys DEV_KEY = Keys.F7;

        public DeveloperMode DeveloperMode { get; private set; } = DeveloperMode.FullOn;

        public DeveloperSwitch(DxGame game) 
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            var inputModel = DxGame.Model<InputModel>();
            var finishedEvents = inputModel.FinishedEvents;
            if (finishedEvents.Any(inputEvent => inputEvent.Key == DEV_KEY))
            {
                DeveloperMode = DeveloperMode.Rotate();
            }

            base.Update(gameTime);
        }
    }
}
