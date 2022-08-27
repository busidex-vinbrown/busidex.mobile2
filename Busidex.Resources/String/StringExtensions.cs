using System;
using System.Linq;

namespace Busidex.Resources.String
{
    public static class StringExtensions
    {
        public static string ToHexString(this string s)
        {
            return string.Join("",
            s.Select(c => string.Format("{0:X2}", Convert.ToInt32(c))));
        }

        public static byte[] FromHex(this string hex)
        {
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
