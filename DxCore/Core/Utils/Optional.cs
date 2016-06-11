using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Utils
{
    /**

        <summary>
            Home-brew port of Java's Optional (https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html)
        </summary>
    */

    [Serializable]
    [DataContract]
    [Obsolete("Optional is deprecated, consider using out values instead")]
    public struct Optional<T>
    {
        private static readonly Optional<T> EMPTY = new Optional<T>(default(T), false);
        public static Optional<T> Empty => EMPTY;

        [DataMember]
        private readonly T value_;

        [DataMember]
        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (HasValue)
                {
                    return value_;
                }
                throw new NullReferenceException();
            }
        }

        private Optional(T value, bool hasValue)
        {
            value_ = value;
            HasValue = hasValue;
        }

        public Optional(T value)
            : this(value, !ReferenceEquals(value, default(T)))
        {
        }

        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }

        public static bool operator ==(Optional<T> lhs, Optional<T> rhs)
        {
            /* 
                If their values are equivalent, by construction, their HasValue attributes are equivalent 
            */
            return Objects.Equals(lhs, rhs);
        }

        public static bool operator !=(Optional<T> lhs, Optional<T> rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            return other is Optional<T> && Equals((Optional<T>) other);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(value_);
        }

        public bool Equals(Optional<T> other)
        {
            return this == other;
        }
    }
}