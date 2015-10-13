using System;

namespace DXGame.Core.Utils
{
    public static class TimeSpanUtils
    {
        public static double Divide(this TimeSpan lhs, TimeSpan rhs)
        {
            var lhsMilliseconds = lhs.TotalMilliseconds;
            var rhsMilliseconds = rhs.TotalMilliseconds;
            return lhsMilliseconds / rhsMilliseconds;
        }
    }
}