using DxCore.Core.Map;
using DxCore.Core.Messaging;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Messaging
{
    public class MapLayoutChanged : Message
    {
        public MapLayout NewLayout { get; }

        public MapLayoutChanged(MapLayout newLayout)
        {
            Validate.Hard.IsNotNull(newLayout);
            NewLayout = newLayout;
        }
    }
}