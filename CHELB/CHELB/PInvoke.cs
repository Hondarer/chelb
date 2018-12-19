using System;
using System.Runtime.InteropServices;

namespace CHELB
{
    public class PInvoke
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, IntPtr count);

        public static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((a == null) || (b == null) || (a.Length != b.Length))
            {
                return false;
            }

            return memcmp(a, b, new IntPtr(a.Length)) == 0;
        }

        public static bool ByteArrayCompareStartWith(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((a == null) || (b == null) || (a.Length > b.Length))
            {
                return false;
            }

            return memcmp(a, b, new IntPtr(a.Length)) == 0;
        }
    }
}
