using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Messaging
{
    public class AnimationChangedMessage : Message
    {
        public AnimationDescriptor Descriptor { get; private set; }

        public AnimationChangedMessage(AnimationDescriptor descriptor)
        {
            Validate.Hard.IsNotNull(descriptor);
            Descriptor = descriptor;
        }
    }
}