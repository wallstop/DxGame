using System;
using System.Collections.Generic;
using AnimationEditorLibrary.Core.Messaging;
using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace AnimationEditorLibrary.Controls
{
    public class AnimationView : ViewModelBase
    {
        private const float BaseScrollScale = 1200f;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool FacingLeft
        {
            get { return Facing == Direction.West; }
            set { ChangeDirection(value ? Direction.West : Direction.East); }
        }

        public bool FacingRight
        {
            get { return Facing == Direction.East; }
            set { ChangeDirection(value ? Direction.East : Direction.West); }
        }

        public int FPS
        {
            get { return Descriptor.FramesPerSecond; }
            set
            {
                Builder.WithFps(value);
                NotifyAnimationChanged();
            }
        }

        public int FrameCount
        {
            get { return Descriptor.FrameCount; }
            set
            {
                Builder.WithFrameCount(value);
                NotifyAnimationChanged();
            }
        }

        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
            new Dictionary<RoutedEvent, Delegate>
            {
                [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(HandleMouseScroll)
            };

        public int Height
        {
            get { return Descriptor.Height; }
            set
            {
                Builder.WithHeight(value);
                NotifyAnimationChanged();
            }
        }

        public int Width
        {
            get { return Descriptor.Width; }
            set
            {
                Builder.WithWidth(value);
                NotifyAnimationChanged();
            }
        }

        private AnimationDescriptor.AnimationDescriptorBuilder Builder { get; }
        private AnimationDescriptor Descriptor => Builder.Build();

        private Direction Facing { get; set; }

        private float Scale { get; set; }

        public AnimationView()
        {
            Builder = AnimationDescriptor.NewBuilder;
            FacingRight = true;
        }

        private void ChangeDirection(Direction direction)
        {
            if(Facing == direction)
            {
                return;
            }
            new OrientationChangedMessage(direction).Emit();
            Facing = direction;
        }

        private void HandleMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            // TODO: Punt to camera model?
            Builder.WithScale(Scale);
            NotifyAnimationChanged();
        }

        private void NotifyAnimationChanged()
        {
            new AnimationChangedMessage(Descriptor).Emit();
        }
    }
}