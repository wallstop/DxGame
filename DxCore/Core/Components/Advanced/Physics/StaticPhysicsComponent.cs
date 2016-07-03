using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public sealed class StaticPhysicsComponent : IDxWorldMember
    {
        public DxVector2 Position { get; set; }
        public float Height { get; }
        public float Width { get; }
        public DxRectangle Bounds { get; }
        public DxRectangle Space { get; }
        public DxVector2 Center { get; }
        public Fixture Fixture { get; }
        public PhysicsType PhysicsType { get; }

        public class StaticPhysicsComponentBuilder : IBuilder<StaticPhysicsComponent>
        {
            private DxVector2? position_;
            private DxVector2? bounds_;
            //private bool gravity

            public StaticPhysicsComponent Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}
