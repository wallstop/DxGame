using System;
using System.Collections.Generic;
using System.Linq;

namespace DxCore.Core.Utils
{
    public static class CollectionUtils
    {
        /**
        <summary>
            Unfortunately, the only kind of C# arrays that implement IEnumerable<T> are single dimsional arrays.
            

            So, in order to do nice LINQ expressions (http://msdn.microsoft.com/en-us/library/vstudio/bb397676%28v=vs.100%29.aspx) 
            on multidimensonal arrays, we have this generator function (which yields, allows continuable calls).

            To use:
            <code>
                myMultiDimensionalArray.ToEnumerable<TypeOfArray>(); 
            </code>
        </summary>
        */

        public static IEnumerable<T> ToEnumerable<T>(this Array target)
        {
            return from object item in target select (T) item;
        }

        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (double) size); ++i)
            {
                yield return new List<T>(source.Skip(size * i).Take(size));
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}