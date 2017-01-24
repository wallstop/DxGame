using DxCore.Core.Messaging;
using DxCore.Core.Primitives;

namespace AnimationEditorLibrary.Core.Messaging
{
    public class SpriteSheetOffsetChangedMessage : Message
    {
        public DxVector2 LatesOffset { get; }

        public SpriteSheetOffsetChangedMessage(DxVector2 offset)
        {
            LatesOffset = offset;
        }
    }
}