using AnimationEditorLibrary.Core.Messaging;
using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace AnimationEditorLibrary.Controls
{
    public sealed class AnimationView : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool facingLeft_;
        private bool facingRight_;

        // TODO: Unify/simplify?
        public bool FacingLeft
        {
            get { return facingLeft_; }
            set
            {
                if(facingLeft_ == value)
                {
                    return;
                }
                if(value)
                {
                    new OrientationChangedMessage(Direction.East).Emit();
                }
                facingLeft_ = value;
            }
        }

        public bool FacingRight
        {
            get { return facingRight_; }
            set
            {
                if(facingRight_ == value)
                {
                    return;
                }
                if(value)
                {
                    new OrientationChangedMessage(Direction.West).Emit();
                }
                facingRight_ = value;
            }
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

        public AnimationView()
        {
            Builder = AnimationDescriptor.NewBuilder;
            FacingRight = true;
        }

        private void NotifyAnimationChanged()
        {
            new AnimationChangedMessage(Descriptor).Emit();
        }
    }
}