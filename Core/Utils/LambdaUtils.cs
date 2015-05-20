using System;
using System.Runtime.CompilerServices;

namespace DXGame.Core.Utils
{
    public static class LambdaUtils
    {
        public static int DelegateHashCode(Delegate method)
        {
            if (method == null)
            {
                return 0;
            }
            int result = method.Method.GetHashCode() ^ method.GetType().GetHashCode();
            if (method.Target != null)
            {
                result ^= RuntimeHelpers.GetHashCode(method);
            }
            return result;
        }
    }
}