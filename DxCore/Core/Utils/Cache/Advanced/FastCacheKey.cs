using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Utils.Cache.Advanced
{
    /**
        <summary>
            Extremely simple / fast Cache Key for use with Single-Element caches
        </summary>
    */
    [DataContract]
    [Serializable]
    public sealed class FastCacheKey : IEquatable<FastCacheKey>
    {
        public static readonly FastCacheKey Instance = new FastCacheKey();

        private FastCacheKey() {}

        public override bool Equals(object other) => Equals(other as FastCacheKey);

        public bool Equals(FastCacheKey other) => !ReferenceEquals(other, null);

        public override string ToString() => nameof(FastCacheKey);

        // Fair dice roll
        public override int GetHashCode() => 9;
    }
}
