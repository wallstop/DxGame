using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Core.Utils
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
            return from object item in target select (T)item;
        }
    }
}
