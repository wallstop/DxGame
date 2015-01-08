using System.Collections.Generic;

namespace DXGame.Core.Utils
{
    public static class GenericUtils
    {
        public static bool IsNullOrDefault<T>(T instance)
        {
            return EqualityComparer<T>.Default.Equals(instance, default(T));
        }
    }
}
