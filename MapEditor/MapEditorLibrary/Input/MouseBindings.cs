using System;
using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Input;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace MapEditorLibrary.Input
{
    public static class MouseBindings
    {
        /* Generates an InputBinding that pans when the provided mouseAction is continually applied (button held down). Stops when the event no longer occurs */

        public static IEnumerable<InputBinding> Pan(MouseAction mouseAction, Action<object> onPanStart, Action<object> onPanEnd)
        {
            // Need to provide engage & disengage
            Validate.Hard.IsNotNullOrDefault(mouseAction); // Can't bind to None, currently
            
            RelayCommand enablePan = new RelayCommand(onPanStart);
            // WTF is up with Modifier Keys - API should take IEnumerable instead of single, fucking stupid
            MouseBinding panStartBind = new MouseBinding(enablePan, mouseAction, ModifierKeys.None);

            RelayCommand disablePan = new RelayCommand(onPanEnd);
            MouseBinding panEndBind = new MouseBinding(disablePan, MouseAction.None, ModifierKeys.None);

            return new[] { panStartBind, panEndBind };
        }
    }
}
