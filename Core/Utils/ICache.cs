using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Utils
{
    /**
        <summary>
            Simple interface that all caches should adhere to.
        </summary>
    */
    public interface ICache<U, T>
    {
        IReadOnlyCollection<T> Elements
        {
            get;
        }

        IReadOnlyDictionary<U, T> KeyedElements
        {
            get;
        }

        T Get(U key);
    }
}
