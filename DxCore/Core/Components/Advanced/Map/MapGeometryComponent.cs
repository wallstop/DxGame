using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace DxCore.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public sealed class MapGeometryComponent : Component
    {
        [DataMember]
        private List<List<DxVector2>> MapGeometry { get; set; }

        [IgnoreDataMember]
        private Body MapGeometryBody { get; set; }

        public MapGeometryComponent(ICollection<DxRectangle> mapTiles)
        {
            Validate.Hard.IsNotNull(mapTiles);
            MapGeometry = mapTiles.Simplify();
        }

        public override void Initialize()
        {
            WorldModel worldModel = DxGame.Instance.Model<WorldModel>();
            MapGeometryBody = BodyFactory.CreateBody(worldModel.World, this);
            foreach(List<DxVector2> shape in MapGeometry)
            {
                Fixture shapeBounds = FixtureFactory.AttachPolygon(shape.Select(point => point * WorldModel.DxToFarseerScale).ToVertices(), 0, MapGeometryBody, this);
                shapeBounds.CollisionCategories = CollisionGroup.Map.CollisionCategory;
            }

            MapGeometryBody.Restitution = 0;
            MapGeometryBody.Friction = 0;
            MapGeometryBody.FixedRotation = true;
            MapGeometryBody.BodyType = BodyType.Static;
            MapGeometryBody.IgnoreGravity = true;
        }

        public override void Remove()
        {
            WorldModel worldModel = DxGame.Instance.Model<WorldModel>();
            worldModel.World.RemoveBody(MapGeometryBody);
            MapGeometryBody = null;
            base.Remove();
        }
    }
}
