using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapCollidable : IIdentifiable
    {
        [DataMember]
        public UniqueId Id { get; }

        [DataMember]
        public Tile Tile { get; }

        [DataMember]
        public DxRectangle Space { get; }

        public MapCollidable(Tile tile, DxRectangle space)
        {
            Validate.IsNotNullOrDefault(tile);
            Validate.IsNotNullOrDefault(space);
            Tile = tile;
            Space = space;
        }
    }
}
