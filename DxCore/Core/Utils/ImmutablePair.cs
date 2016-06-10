using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils
{
    [Serializable]
    [DataContract]
    public struct ImmutablePair<T, U> : IPair<T, U>
    {
        public ImmutablePair(T key, U value)
        {
            Key = key;
            Value = value;
        }

        public T Key { get; }
        public U Value { get; }

        public static bool operator !=(ImmutablePair<T, U> lhs, IPair<T, U> rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(ImmutablePair<T, U> lhs, IPair<T, U> rhs)
        {
            return Objects.Equals(lhs.Key, rhs.Key) && Objects.Equals(lhs.Value, rhs.Value);
        }

        public override bool Equals(object other)
        {
            return other is IPair<T, U> && (this == (IPair<T, U>) other);
        }

        public bool Equals(IPair<T, U> other)
        {
            return (this == other);
        }

        public override string ToString()
        {
            return $"{{ {Key}, {Value} }}";
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Key, Value);
        }
    }
}