using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Physics
{
    [Serializable]
    [DataContract]
    public sealed class Force
    {
        [DataMember]
        public DxVector2 Value { get; private set; }

        public Force(DxVector2 force)
        {
            Value = force * DxGame.Instance.CurrentUpdateTime.ScaleFactor;
        }
    }
}
