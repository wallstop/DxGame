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
    }
}