using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Map;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public class MapCollidableComponent : CollidableComponent
    {
        [DataMember]
        public PlatformType PlatformType { get; private set; }

        public MapCollidableComponent(DxGame game, PlatformType type) 
            : base(game)
        {
            PlatformType = type;
        }
    }
}
