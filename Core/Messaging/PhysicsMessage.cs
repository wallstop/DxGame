using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class PhysicsMessage : Message
    {
        [DataMember]
        public GameObject Source { get; set; }

        [DataMember]
        public List<IShape> AffectedAreas { get; set; } = new List<IShape>();

        [DataMember]
        public Interaction Interaction { get; set; }
    }
}