using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;

namespace MapEditorLibrary.Core.Components
{
    public class MapCreatorComponent : Component
    {
        private MapDescriptor.MapDescriptorBuilder MapBuilder { get; }

        public MapCreatorComponent()
        {
            MapBuilder = new MapDescriptor.MapDescriptorBuilder();
        }
    }
}
