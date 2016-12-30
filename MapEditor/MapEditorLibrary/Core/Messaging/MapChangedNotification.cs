using DxCore.Core.Map;
using DxCore.Core.Messaging;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Messaging
{
    public class MapChangedNotification : Message
    {
        public MapDescriptor MapDescriptor { get; }

        public MapChangedNotification(MapDescriptor mapDescriptor)
        {
            Validate.Hard.IsNotNull(mapDescriptor);
            MapDescriptor = mapDescriptor;
        }
    }
}