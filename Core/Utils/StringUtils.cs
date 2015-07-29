using System.Collections.Generic;

namespace DXGame.Core.Utils
{
    public static class StringUtils
    {
        public static byte[] GetBytes(string input)
        {
            byte[] bytes = new byte[input.Length * sizeof (char)];
            System.Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] characters = new char[bytes.Length / sizeof (char)];
            System.Buffer.BlockCopy(bytes, 0, characters, 0, bytes.Length);
            return new string(characters);
        }

        public static string GetFormattedNullDefaultMessage<T, U>(T instance, U argument)
        {
            return $"Cannot initialize a {typeof (T)} with a null/default {typeof (U)}";
        }

        public static string GetFormattedAlreadyContainsMessage<T, U>(T instance, U argument, IEnumerable<U> collection)
        {
            return $"Cannot add a {typeof (U)} to a {typeof (T)}, as one already exists ({collection})";
        }
    }
}