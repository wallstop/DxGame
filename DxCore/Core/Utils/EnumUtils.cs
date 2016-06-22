using System;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils
{
    /**
        <summary> Various utility methods for C#s build-in enum classes/types</summary>
    */

    public static class EnumUtils
    {
        /**

            <summary> Rotate's an enum's value to the next one, or the first one if the value is the last. </summary>
        */

        public static T Rotate<T>(this T currentValue) where T : struct
        {
            Validate.Validate.Hard.IsTrue(typeof (T).IsEnum, "Cannot rotate a non-enum type");
            T[] enumValues = (T[]) Enum.GetValues(currentValue.GetType());
            int nextIndex = (Array.IndexOf(enumValues, currentValue) + 1) % enumValues.Length;
            return enumValues[nextIndex];
        }
    }
}