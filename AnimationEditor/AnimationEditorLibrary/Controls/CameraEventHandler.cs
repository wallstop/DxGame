using System;
using System.Collections.Generic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Camera;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;

namespace AnimationEditorLibrary.Controls
{
    public class CameraEventHandler
    {
        private const float BaseScrollScale = 1200f;

        // TODO: Pull out to common core library for this kind of shit?
        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
            new Dictionary<RoutedEvent, Delegate>
            {
                [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(HandleMouseScroll)
            };

        private float Scale { get; set; }

        public CameraEventHandler()
        {
            Scale = 1.0f;
        }

        private void HandleMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            // TODO: Punt to camera model?
            new ZoomRequest(Scale).Emit();
        }
    }
}