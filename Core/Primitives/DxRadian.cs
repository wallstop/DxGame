using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Primitives
{
    /**
        Follows the XNA Coordinate Space

              -1 y
               |
               |
       -1______|______1 x
               |
               |
               1 

        <summary> Basic Radian class for Math functions (and stuff).</summary>
    */

    [Serializable]
    [DataContract]
    public struct DxRadian
    {
        [DataMember] public float Value;
        public DxDegree Degree => new DxDegree(this);
        /* What the shit? http://stackoverflow.com/questions/2276855/xna-2d-vector-angles-whats-the-correct-way-to-calculate */
        public DxVector2 UnitVector => new DxVector2((float) Math.Sin(Value), -(float) Math.Cos(Value));

        public DxRadian(double value)
            : this((float) (value))
        {
        }

        public DxRadian(float value)
        {
            Value = value;
        }

        public DxRadian(DxRadian radian)
        {
            Value = radian.Value;
        }

        public DxRadian(DxDegree degree)
        {
            Value = (float) (Math.PI * degree.Value / 180.0);
        }

        public DxRadian(DxVector2 vector)
        {
            Value = (float) Math.Atan2(vector.X, -vector.Y);
        }
    }
}