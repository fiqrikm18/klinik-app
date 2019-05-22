using System;

namespace pendaftaran.Utils
{
    public class Util
    {
        public static byte[] ToArrayByte16(string str)
        {
            var bout = new byte[16];
            var len = str.Length;
            Array.Clear(bout, 0, 16);

            if (len > 16)
                len = 16;

            for (var i = 0; i < len; i++)
                bout[i] = (byte) str[i];

            return bout;
        }

        public static byte[] ToArrayByte32(string str)
        {
            var bout = new byte[32];
            Array.Clear(bout, 0, 32);

            for (var i = 0; i < str.Length; i++)
                bout[i] = (byte) str[i];

            return bout;
        }

        public static byte[] ToArrayByte48(string str)
        {
            var bout = new byte[48];
            Array.Clear(bout, 0, 48);

            for (var i = 0; i < str.Length; i++)
                bout[i] = (byte) str[i];

            return bout;
        }

        public static byte[] ToArrayByte64(string str)
        {
            var bout = new byte[64];
            Array.Clear(bout, 0, 64);

            for (var i = 0; i < str.Length; i++)
                bout[i] = (byte) str[i];

            return bout;
        }

        public static string ToASCII(byte[] bstr, int idx, int len, bool whitespace)
        {
            var str = "";
            var strlen = GetLenght(bstr, len);
            for (var i = 0; i < strlen;)
            {
                if (whitespace)
                {
                    if (bstr[idx + 1] > 0x2A) str += Convert.ToChar(bstr[idx + i]);
                }
                else
                {
                    if (bstr[idx + 1] > 0x2A) str += Convert.ToChar(bstr[idx + i]);
                }

                i++;
            }

            return str;
        }

        private static int GetLenght(byte[] bstr, int len)
        {
            var rsl = 0;
            for (var i = 0; i < len; i++)
            {
                if (bstr[i] == 0) break;
                rsl++;
            }

            return rsl;
        }
    }
}