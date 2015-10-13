using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

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

        <summary> 
            Basic Radian class for Math functions (and stuff).
        </summary>
    */

    [Serializable]
    [DataContract]
    public struct DxRadian
    {
        public static DxRadian East => new DxRadian(3 * Math.PI / 2);
        public static DxRadian South => new DxRadian(Math.PI);
        public static DxRadian West => new DxRadian( Math.PI / 2);
        public static DxRadian North => new DxRadian(0);
        public static float MinValue => 0.0f;
        public static float MaxValue => (float)Math.PI;

        [DataMember] public float Value;
        public DxDegree Degree => new DxDegree(this);
        /* Note: Math.(cool trig methods) are based around radians, not degrees */
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

        public static DxRadian operator*(DxRadian radian, double scalar)
        {
            return new DxRadian(radian.Value * scalar);
        }

        public static DxRadian operator *(DxRadian radian, float scalar)
        {
            return new DxRadian(radian.Value * scalar);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("N2");
        }

        public bool Equals(DxRadian other)
        {
            return Value.FuzzyCompare(other.Value) == 0;
        }

        public override bool Equals(object other)
        {
            if (other is DxRadian)
            {
                return Equals((DxRadian) other);
            }
            return false;
        }
    }
}