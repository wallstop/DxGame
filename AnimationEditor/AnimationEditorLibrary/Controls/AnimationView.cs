using AnimationEditorLibrary.Core.Messaging;
using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace AnimationEditorLibrary.Controls
{
    public sealed class AnimationView : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int FPS
        {
            get { return Descriptor.FramesPerSecond; }
            set
            {
                Descriptor.FramesPerSecond = value;
                NotifyAnimationChanged();
            }
        }

        public int FrameCount
        {
            get { return Descriptor.FrameCount; }
            set
            {
                int oldFrameCount = Descriptor.FrameCount;
                Descriptor.FrameCount = value;
                if(oldFrameCount < value)
                {
                    for(int i = oldFrameCount; i < value; ++i)
                    {
                        Builder.WithFrameOffset(i, FrameDescriptor.NewFrameDescriptor);
                    }
                }
                else
                {
                    for(int i = value; i < oldFrameCount; ++i)
                    {
                        Builder.WithoutFrameOffset(i);
                    }
                }
                NotifyAnimationChanged();
            }
        }

        public int Height
        {
            get { return (int) Descriptor.BoundingBox.Height; }
            set
            {
                DxRectangle bounds = Descriptor.BoundingBox;
                bounds.Height = value;

                Descriptor.BoundingBox = bounds;
                SyncBoundingBox();
                NotifyAnimationChanged();
            }
        }

        public int Width
        {
            get { return (int) Descriptor.BoundingBox.Width; }
            set
            {
                DxRectangle bounds = Descriptor.BoundingBox;
                bounds.Width = value;

                Descriptor.BoundingBox = bounds;
                SyncBoundingBox();
                NotifyAnimationChanged();
            }
        }

        private AnimationFrameOffset.AnimationFrameOffsetBuilder Builder { get; set; }
        private AnimationDescriptor Descriptor { get; set; }

        public AnimationView()
        {
            Builder = new AnimationFrameOffset.AnimationFrameOffsetBuilder();
            Descriptor = AnimationDescriptor.Empty();
        }

        private void NotifyAnimationChanged()
        {
            SyncDescriptorWithOffsets();
            AnimationDescriptor latestDescriptor = Descriptor;

            new AnimationChangedMessage(latestDescriptor).Emit();
        }

        private void SyncBoundingBox()
        {
            for(int i = 0; i < Descriptor.FrameCount; ++i)
            {
                DxVector2 drawOffset;
                DxVector2 frameOffset;
                DxRectangle frameBounds;
                if(Descriptor.FrameOffsets.OffsetForFrame(i, out frameOffset, out drawOffset, out frameBounds))
                {
                    FrameDescriptor updatedDescriptor = new FrameDescriptor
                    {
                        BoundingBox = Descriptor.BoundingBox,
                        DrawOffset = drawOffset,
                        FrameOffset = frameOffset
                    };
                    Builder.WithFrameOffset(i, updatedDescriptor);
                }
                else
                {
                    Logger.Error("No {0} found for {1}", nameof(AnimationFrameOffset), i);
                }
            }
        }

        private void SyncDescriptorWithOffsets()
        {
            Descriptor.FrameOffsets = Builder.Build();
        }
    }
}