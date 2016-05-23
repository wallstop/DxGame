using System.Collections.Generic;
using System.Linq;

namespace DXGame.Core.Utils
{
    public static class DictionaryUtils
    {
        public static Dictionary<T, V> ToDictionary<T, V>(this Dictionary<T, V> dictionary)
        {
            return dictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }
}
