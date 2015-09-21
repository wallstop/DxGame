using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Physics;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class PhysicsMessage : Message
    {
        [DataMember]
        public GameObject Source { get; set; }

        [DataMember]
        public List<DxRectangle> AffectedAreas { get; set; } = new List<DxRectangle>();

        [DataMember]
        public Interaction Interaction { get; set; }
    }
}