using System.Collections.Generic;
using System.Diagnostics;

namespace DXGame.Core.Utils
{
    public static class GenericUtils
    {
        public static bool IsNullOrDefault<T>(T instance)
        {
            return EqualityComparer<T>.Default.Equals(instance, default(T));
        }

        public static void CheckNullOrDefault<T>(T instance, string message = "")
        {
            Debug.Assert(!IsNullOrDefault(instance), message);
        }

        public static void CheckNull<T>(T instance, string message = "")
        {
            Debug.Assert(null != instance, message);
        }
    }
}
