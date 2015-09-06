using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Map;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    public class MapCollidableComponent : CollidableComponent
    {
        public PlatformType PlatformType { get; }

        public MapCollidableComponent(DxGame game, PlatformType type) 
            : base(game)
        {
            PlatformType = type;
        }
    }
}
