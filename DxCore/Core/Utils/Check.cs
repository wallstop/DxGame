using System.Collections.Generic;

namespace DxCore.Core.Utils
{
    public static class Check
    {
        public static bool IsNullOrDefault<T>(T instance)
        {
            return EqualityComparer<T>.Default.Equals(instance, default(T));
        }

        public static bool IsNotNullOrDefault<T>(T instance)
        {
            return !IsNullOrDefault(instance);
        }
    }
}