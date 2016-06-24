using System;
using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Input;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace MapEditorLibrary.Input
{
    public static class MouseDrivenMapNavigation
    {
        /* Generates an InputBinding that pans when the provided mouseAction is continually applied (button held down). Stops when the event no longer occurs */

        public static IEnumerable<InputBinding> Pan(MouseAction mouseAction, out Func<DxVector2> displacementFunction)
        {
            // Need to provide engage & disengage
            Validate.Hard.IsNotNullOrDefault(mouseAction); // Can't bind to None, currently

            PanManager panManager = new PanManager();
            RelayCommand enablePan = new RelayCommand(doesntMatter => { panManager.Enabled = true; });
            // WTF is up with Modifier Keys - API should take IEnumerable instead of single, fucking stupid
            MouseBinding panStart = new MouseBinding(enablePan, mouseAction, ModifierKeys.None);

            RelayCommand disablePan = new RelayCommand(doesntMatter => { panManager.Enabled = false; });
            MouseBinding panEnd = new MouseBinding(disablePan, MouseAction.None, ModifierKeys.None);

            displacementFunction = panManager.Displacement;
            return new[] {panStart, panEnd};
        }

        internal sealed class PanManager
        {
            private DxVector2 Start { get; set; }

            private bool enabled_;

            public bool Enabled
            {
                get { return enabled_; }
                set
                {
                    if(value)
                    {
                        Start = Mouse.GetState().Position;
                    }
                    enabled_ = value;
                }
            }

            public PanManager()
            {
                Enabled = false;
            }

            public DxVector2 Displacement()
            {
                if(!Enabled)
                {
                    return DxVector2.EmptyVector;
                }

                DxVector2 current = Mouse.GetState().Position;
                DxVector2 displacement = current - Start;
                Start = current;
                return displacement;
            }
        }
    }
}
