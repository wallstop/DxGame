using System.Collections.Generic;

namespace DxCore.Core.Utils.Cache.Simple
{
    /**
        <summary>
            Simple interface that all caches should adhere to.
        </summary>
    */
    public interface ISimpleCache<U, T>
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
