using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;

namespace AnimationEditorLibrary.Core.Messaging
{
    public sealed class OrientationChangedMessage : Message
    {
        public Direction Orientation { get; }

        public OrientationChangedMessage(Direction direction)
        {
            Orientation = direction;
        }
    }
}