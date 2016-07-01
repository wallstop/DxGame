using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Physics
{
    [Serializable]
    [DataContract]
    public sealed class Nullification
    {
        public static readonly Nullification Vertical = new Nullification(Axis.Y);
        public static readonly Nullification Horizontal = new Nullification(Axis.X);

        [DataMember]
        public Axis Axis { get; private set; }

        private Nullification(Axis axis)
        {
            Validate.Hard.IsNotNullOrDefault(axis);
            Axis = axis;
        }
    }
}
