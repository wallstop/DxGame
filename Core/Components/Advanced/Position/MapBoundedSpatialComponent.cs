using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Components.Advanced.Position
{
    [Serializable]
    [DataContract]
    public class MapBoundedSpatialComponent : BoundedSpatialComponent
    {
        [IgnoreDataMember]
        public override DxRectangle Bounds => DxGame.Instance.Model<MapModel>().MapBounds;

        public MapBoundedSpatialComponent(DxVector2 position, DxVector2 dimensions)
            : base(position, dimensions)
        {
        }
    }
}
