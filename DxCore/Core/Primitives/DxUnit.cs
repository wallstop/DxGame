using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Primitives
{
    /**
        <summary>
            Base world unit (similar to meters or feet)
        </summary>
        TODO: Actually use throughout gam
    */

    [Serializable]
    [DataContract]
    public struct DxUnit
    {
        public float Value { get; }

        public DxUnit(float value)
        {
            Value = value;
        }
    }
}
