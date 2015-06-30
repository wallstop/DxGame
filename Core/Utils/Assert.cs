﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DXGame.Core.Utils
{
    public static class Assert
    {
        private static void FailIfTrue(bool expression, string message)
        {
            Debug.Assert(expression, message);
        }

        public static void IsTrue(bool expression)
        {
            IsTrue(expression, "Asserted expression was false");
        }

        public static void IsTrue(bool expression, string message)
        {
            FailIfTrue(!expression, message);
        }

        public static void IsFalse(bool expression)
        {
            IsFalse(expression, "Expected False, validated expresssion was True");
        }

        public static void IsFalse(bool expression, string message)
        {
            FailIfTrue(expression, message);
        }

        public static void IsInClosedInterval<T>(T value, T min, T max) where T : IComparable<T>
        {
            IsInClosedInterval(value, min, max, $"{value} is not within [{min}, {max}]");
        }

        public static void IsInClosedInterval<T>(T value, T min, T max, string message) where T : IComparable<T>
        {
            FailIfTrue(value.CompareTo(min) < 0 || value.CompareTo(max) > 0, message);
        }

        public static void IsInOpenInterval<T>(T value, T min, T max) where T : IComparable<T>
        {
            IsInOpenInterval(value, min, max, $"{value} is not within ({min}, {max})");
        }

        public static void IsInOpenInterval<T>(T value, T min, T max, string message) where T : IComparable<T>
        {
            FailIfTrue(value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0, message);
        }

        public static void IsNotNull<T>(T value)
        {
            IsNotNull(value, "Validated object was null");
        }

        public static void IsNotNull<T>(T value, string message)
        {
            FailIfTrue(null == value, message);
        }

        public static void IsNull<T>(T value)
        {
            IsNull(value, "Valdited object was not null");
        }

        public static void IsNull<T>(T value, string message)
        {
            FailIfTrue(null != value, message);
        }

        public static void IsNotEmpty<T>(IEnumerable<T> enumeration)
        {
            IsNotEmpty(enumeration, "Validated enumeration was empty");
        }

        public static void IsNotEmpty<T>(IEnumerable<T> enumeration, string message)
        {
            FailIfTrue(enumeration == null || !enumeration.Any(), message);
        }

        public static void IsEmpty<T>(IEnumerable<T> enumeration)
        {
            IsEmpty(enumeration, $"Validated enumeration {enumeration} was not empty");
        }

        public static void IsEmpty<T>(IEnumerable<T> enumeration, string message)
        {
            FailIfTrue(null == enumeration || enumeration.Any(), message);
        }

        public static void IsNotNullOrDefault<T>(T value)
        {
            IsNotNullOrDefault(value, $"{value} was null or default for {typeof (T)}");
        }

        public static void IsNotNullOrDefault<T>(T value, string message)
        {
            FailIfTrue(EqualityComparer<T>.Default.Equals(value, default(T)), message);
        }
    }
}