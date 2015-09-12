using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Utils
{
    public static class Objects
    {
        public static int HashCode(params object[] args)
        {
            unchecked
            {
                /* Borrowed from http://stackoverflow.com/questions/5183929/comparing-two-objects, ez */
                return args.Aggregate((int)2166136261, (current, arg) => current * 16777619 ^ arg.GetHashCode());
            }
        }

        public static bool Equals<T, U>(T first, U second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            return !ReferenceEquals(first, null) && !ReferenceEquals(second, null) && first.Equals(second);
        }
    }
}
