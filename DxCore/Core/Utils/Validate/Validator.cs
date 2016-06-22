using System;
using System.Collections.Generic;
using System.Linq;

namespace DxCore.Core.Utils.Validate
{
    public abstract class Validator
    {
        protected const string DefaultMessage = "assertion failed";

        protected abstract void TestFailure(Func<string> messageProducer);

        protected void FailIfFalse(bool expression, Func<string> messageProducer)
        {
            if(!expression)
            {
                TestFailure(messageProducer);
            }
        }

        public bool IsTrue(bool expression) => IsTrue(expression, DefaultMessage);
        public bool IsTrue(bool expression, string message) => IsTrue(expression, () => message);

        public bool IsTrue(bool expression, Func<string> messageProducer)
        {
            FailIfFalse(expression, messageProducer);
            return expression;
        }

        public bool IsFalse(bool expression) => !IsTrue(expression);
        public bool IsFalse(bool expression, string message) => !IsTrue(expression, message);
        public bool IsFalse(bool expression, Func<string> messageProducer) => !IsTrue(expression, messageProducer);

        public bool IsInClosedInterval<T>(T value, T min, T max) where T : IComparable<T>
            => IsInClosedInterval(value, min, max, DefaultMessage);

        public bool IsInClosedInterval<T>(T value, T min, T max, string message) where T : IComparable<T>
            => IsInClosedInterval(value, min, max, () => message);

        public bool IsInClosedInterval<T>(T value, T min, T max, Func<string> messageProducer) where T : IComparable<T>
        {
            bool inClosedInterval = 0 <= value.CompareTo(min) && value.CompareTo(max) <= 0;
            FailIfFalse(inClosedInterval, messageProducer);
            return inClosedInterval;
        }

        public bool IsInOpenInterval<T>(T value, T min, T max) where T : IComparable<T>
            => IsInOpenInterval(value, min, max, DefaultMessage);

        public bool IsInOpenInterval<T>(T value, T min, T max, string message) where T : IComparable<T>
            => IsInOpenInterval(value, min, max, () => message);

        public bool IsInOpenInterval<T>(T value, T min, T max, Func<string> messageProducer) where T : IComparable<T>
        {
            bool inOpenInterval = 0 < value.CompareTo(min) && value.CompareTo(max) < 0;
            FailIfFalse(inOpenInterval, messageProducer);
            return inOpenInterval;
        }

        public bool IsNull<T>(T value) => IsNull(value, DefaultMessage);
        public bool IsNull<T>(T value, string message) => IsNull(value, () => message);

        public bool IsNull<T>(T value, Func<string> messageProducer)
        {
            bool isNull = ReferenceEquals(value, null);
            FailIfFalse(isNull, messageProducer);
            return isNull;
        }

        public bool IsNotNull<T>(T value) => IsNotNull(value, DefaultMessage);
        public bool IsNotNull<T>(T value, string message) => IsNotNull(value, () => message);

        public bool IsNotNull<T>(T value, Func<string> messageProducer)
        {
            bool isNotNull = !ReferenceEquals(value, null);
            FailIfFalse(isNotNull, messageProducer);
            return isNotNull;
        }

        public bool IsNullOrDefault<T>(T value) => IsNullOrDefault(value, DefaultMessage);
        public bool IsNullOrDefault<T>(T value, string message) => IsNullOrDefault(value, () => DefaultMessage);

        public bool IsNullOrDefault<T>(T value, Func<string> messageProducer)
        {
            bool isNullOrDefault = EqualityComparer<T>.Default.Equals(value, default(T));
            FailIfFalse(isNullOrDefault, messageProducer);
            return isNullOrDefault;
        }

        public bool IsNotNullOrDefault<T>(T value) => IsNotNullOrDefault(value, DefaultMessage);
        public bool IsNotNullOrDefault<T>(T value, string message) => IsNotNullOrDefault(value, () => message);

        public bool IsNotNullOrDefault<T>(T value, Func<string> messgaeProducer)
        {
            bool isNotNullOrDefault = !EqualityComparer<T>.Default.Equals(value, default(T));
            FailIfFalse(isNotNullOrDefault, messgaeProducer);
            return isNotNullOrDefault;
        }

        public bool IsEmpty<T>(IEnumerable<T> enumeration) => IsEmpty(enumeration, DefaultMessage);
        public bool IsEmpty<T>(IEnumerable<T> enumeration, string message) => IsEmpty(enumeration, () => message);

        public bool IsEmpty<T>(IEnumerable<T> enumeration, Func<string> messageProducer)
        {
            bool isEmpty = !enumeration.Any();
            FailIfFalse(isEmpty, messageProducer);
            return isEmpty;
        }

        public bool IsNotEmpty<T>(IEnumerable<T> enumeration) => IsNotEmpty(enumeration, DefaultMessage);
        public bool IsNotEmpty<T>(IEnumerable<T> enumeration, string message) => IsNotEmpty(enumeration, () => message);

        public bool IsNotEmpty<T>(IEnumerable<T> enumeration, Func<string> messageProducer)
        {
            bool isNotEmpty = enumeration.Any();
            FailIfFalse(isNotEmpty, messageProducer);
            return isNotEmpty;
        }

        public bool NoNullElements<T>(IEnumerable<T> enumeration) => NoNullElements(enumeration, DefaultMessage);

        public bool NoNullElements<T>(IEnumerable<T> enumeration, string message)
            => NoNullElements(enumeration, () => message);

        public bool NoNullElements<T>(IEnumerable<T> enumeration, Func<string> messageProducer)
        {
            bool noNullElements = enumeration.Any(element => ReferenceEquals(element, null));
            FailIfFalse(noNullElements, messageProducer);
            return noNullElements;
        }

        public bool AreEqual<T>(T first, T second) where T : IEquatable<T> => AreEqual(first, second, DefaultMessage);

        public bool AreEqual<T>(T first, T second, string message) where T : IEquatable<T>
            => AreEqual(first, second, () => message);

        public bool AreEqual<T>(T first, T second, Func<string> messageProducer) where T : IEquatable<T>
        {
            bool areEqual = Objects.Equals(first, second);
            FailIfFalse(areEqual, messageProducer);
            return areEqual;
        }

        public bool AreNotEqual<T>(T first, T second) where T : IEquatable<T>
            => AreNotEqual(first, second, DefaultMessage);

        public bool AreNotEqual<T>(T first, T second, string message) where T : IEquatable<T>
            => AreNotEqual(first, second, () => message);

        public bool AreNotEqual<T>(T first, T second, Func<string> messageProducer) where T : IEquatable<T>
        {
            bool areNotEqual = !Objects.Equals(first, second);
            FailIfFalse(areNotEqual, messageProducer);
            return areNotEqual;
        }
    }
}
