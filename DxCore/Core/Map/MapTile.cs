using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using WallNetCore.Validate;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapTile : IIdentifiable
    {
        [DataMember]
        public DxRectangle Space { get; private set; }

        [DataMember]
        public Tile Tile { get; private set; }

        public MapTile(Tile tile, DxRectangle space)
        {
            Validate.Hard.IsNotNullOrDefault(tile);
            Validate.Hard.IsNotNullOrDefault(space);
            Id = new UniqueId();
            Tile = tile;
            Space = space;
        }

        [DataMember]
        public UniqueId Id { get; private set; }
    }
}