namespace DxCore.Core.Utils
{
    public static class UintUtils
    {
        public static byte A(this uint value)
        {
            return (byte)(value >> 24);
        }

        public static byte B(this uint value)
        {
            return (byte) (value >> 16);
        }

        public static byte C(this uint value)
        {
            return (byte) (value >> 8);
        }

        public static byte D(this uint value)
        {
            return (byte) (value);
        }
    }
}
