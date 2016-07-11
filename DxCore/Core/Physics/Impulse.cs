using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Physics
{
    [Serializable]
    [DataContract]
    public sealed class Impulse
    {
        [DataMember]
        public DxVector2 Value { get; private set; }

        public Impulse(DxVector2 value)
        {
            Value = value;
        }

        public override string ToString() => this.ToJson();
    }
}
