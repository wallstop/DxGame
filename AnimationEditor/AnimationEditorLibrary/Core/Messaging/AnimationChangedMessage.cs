using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Messaging
{
    public class AnimationChangedMessage : Message
    {
        public string AssetPath { get; private set; }

        public AnimationDescriptor Descriptor { get; private set; }

        // AssetPath is nullable
        public AnimationChangedMessage(string assetPath, AnimationDescriptor descriptor)
        {
            AssetPath = assetPath;
            Validate.Hard.IsNotNull(descriptor);
            Descriptor = descriptor;
        }
    }
}