﻿using System;
using System.Collections.Generic;

namespace DXGame.Core.Utils
{
    public static class StringUtils
    {
        public static byte[] GetBytes(string input)
        {
            byte[] bytes = new byte[input.Length * sizeof (char)];
            Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] characters = new char[bytes.Length / sizeof (char)];
            Buffer.BlockCopy(bytes, 0, characters, 0, bytes.Length);
            return new string(characters);
        }

        public static string GetFormattedNullOrDefaultMessage<T>(Type type, T argument)
        {
            return GetFormattedNullOrDefaultMessage(type, $"{typeof (T)}");
        }

        public static string GetFormattedNullOrDefaultMessage(Type type, string argument)
        {
            return $"Cannot initialize a {type} with a null/default {argument}";
        }

        public static string GetFormattedNullOrDefaultMessage<T>(T instance, string argument)
        {
            return GetFormattedNullOrDefaultMessage(typeof (T), argument);
        }

        public static string GetFormattedNullOrDefaultMessage<T, U>(T instance, U argument)
        {
            return GetFormattedNullOrDefaultMessage(typeof (T), argument);
        }

        public static string GetFormattedAlreadyContainsMessage<T, U>(T instance, U argument, IEnumerable<U> collection)
        {
            return $"Cannot add a {typeof (U)} to a {typeof (T)}, as one already exists ({collection})";
        }
    }
}