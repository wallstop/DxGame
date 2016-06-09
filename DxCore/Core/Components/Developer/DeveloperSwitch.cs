﻿using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Developer
{
    public enum DeveloperMode
    {
        FullOn,
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
            var inputModel = DxGame.Instance.Model<InputModel>();
            var finishedEvents = inputModel.FinishedEvents;
            if (finishedEvents.Any(inputEvent => inputEvent.Key == DEV_KEY))
            {
                DeveloperMode = DeveloperMode.Rotate();
            }

            base.Update(gameTime);
        }
    }
}