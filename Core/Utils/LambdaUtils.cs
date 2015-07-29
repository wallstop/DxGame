using System;
using System.Collections.Generic;
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

        public class LambdaComparer<T> : IComparer<T>
        {
            private readonly Func<T, T, int> lambdaComparer_;

            public LambdaComparer(Func<T, T, int> lambdaComparer)
            {
                Validate.IsNotNull(lambdaComparer,
                    StringUtils.GetFormattedNullDefaultMessage(this, nameof(lambdaComparer)));
                lambdaComparer_ = lambdaComparer;
            }

            public int Compare(T lhs, T rhs)
            {
                return lambdaComparer_(lhs, rhs);
            }
        }
    }
}