using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace DxCore.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public sealed class MapGeometryComponent : Component, IWorldCollidable
    {
        [DataMember]
        private List<DxLineSegment> MapGeometry { get; set; }

        [IgnoreDataMember]
        private Body MapGeometryBody { get; set; }

        public MapGeometryComponent(ICollection<DxRectangle> mapTiles)
        {
            Validate.Hard.IsNotNull(mapTiles);
            MapGeometry = mapTiles.Edges();
        }

        public override void Initialize()
        {
            WorldModel worldModel = DxGame.Instance.Model<WorldModel>();
            MapGeometryBody = BodyFactory.CreateBody(worldModel.World, userData:this);
            foreach(DxLineSegment edge in MapGeometry)
            {
                EdgeShape edgeShape = new EdgeShape(edge.Start.Vector2 * WorldModel.DxToFarseerScale,
                    edge.End.Vector2 * WorldModel.DxToFarseerScale);
                MapGeometryBody.CreateFixture(edgeShape, this);
            }

            MapGeometryBody.CollisionCategories = CollisionGroup.Map;
            MapGeometryBody.CollidesWith = CollisionGroup.All;
            MapGeometryBody.SleepingAllowed = false;
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
