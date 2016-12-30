using WallNetCore.Validate;

namespace DxCore.Core.Messaging.Camera
{
    // Not serialized on purpose - these should be local only

    // TODO: Come up with a better way to handle local-only messages (by type)
    public class ZoomRequest : Message
    {
        public float ZoomLevel { get; }

        public ZoomRequest(float zoomLevel)
        {
            Validate.Hard.IsPositive(zoomLevel);
            ZoomLevel = zoomLevel;
        }
    }
}