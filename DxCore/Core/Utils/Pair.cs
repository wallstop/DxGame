using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Utils
{
    [Serializable]
    [DataContract]
    public struct Pair<T, U> : IPair<T, U>, IEquatable<IPair<T, U>>
    {
        public Pair(T key, U value)
        {
            Key = key;
            Value = value;
        }

        public bool Equals(IPair<T, U> other)
        {
            throw new NotImplementedException();
        }

        public T Key { get; }
        public U Value { get; }

        public static bool operator !=(Pair<T, U> lhs, IPair<T, U> rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Pair<T, U> lhs, IPair<T, U> rhs)
        {
            return Objects.Equals(lhs.Key, rhs.Key) && Objects.Equals(lhs.Value, rhs.Value);
        }

        public override bool Equals(object other)
        {
            return other is IPair<T, U> && (this == (IPair<T, U>) other);
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