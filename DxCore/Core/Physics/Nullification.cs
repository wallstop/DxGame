using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Physics
{
    [Serializable]
    [DataContract]
    public sealed class Nullification
    {
        [DataMember]
        public DxVector2 Value { get; private set; }

        public Nullification(DxVector2 maxForceToNullify)
        {
            Value = maxForceToNullify;
        }
    }
}
