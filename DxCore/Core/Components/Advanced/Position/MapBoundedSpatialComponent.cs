using System;
using System.Runtime.Serialization;
using DxCore.Core.Models;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Position
{
    [Serializable]
    [DataContract]
    public class MapBoundedSpatialComponent : BoundedSpatialComponent
    {
        [IgnoreDataMember]
        public override DxRectangle Bounds => DxGame.Instance.Model<MapModel>()?.MapBounds ?? DxGame.Instance.Model<CameraModel>().Bounds;

        public MapBoundedSpatialComponent(DxVector2 position, DxVector2 dimensions) : base(position, dimensions) {}
    }
}
