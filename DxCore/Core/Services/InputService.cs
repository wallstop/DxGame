﻿using DxCore.Core.Input;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    public sealed class InputService : DxService
    {
        public InputHandler InputHandler { get; private set; }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(InputHandler))
            {
                InputHandler = new InputHandler();
                Self.AttachComponent(InputHandler);
            }
        }
    }
}