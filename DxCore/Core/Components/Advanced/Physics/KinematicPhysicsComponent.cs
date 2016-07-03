using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public sealed class KinematicPhysicsComponent : IDxWorldMember
    {
        [DataMember]
        public DxVector2 Position { get; set; }
        public DxRectangle Bounds { get; }
        public DxRectangle Space { get; }
        public DxVector2 Center { get; }
        public Fixture Fixture { get; }
        public PhysicsType PhysicsType { get; }
    }
}
