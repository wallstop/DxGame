using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Primitives
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
