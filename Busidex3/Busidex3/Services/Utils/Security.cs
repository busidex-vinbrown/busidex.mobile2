using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Busidex3.Services.Utils
{
    public class Security
    {
        public static long DecodeUserId(string id)
        {
            byte[] raw = Convert.FromBase64String(id);
            string s = Encoding.UTF8.GetString(raw);
            long.TryParse(s, out var userId);

            return userId;
        }

        public static string EncodeUserId (long userId)
        {
            byte [] toEncodeAsBytes = Encoding.ASCII.GetBytes (userId.ToString (CultureInfo.InvariantCulture));
            string returnValue = Convert.ToBase64String (toEncodeAsBytes);
            return returnValue;
        }

        public static char [] GetDigits (string text)
        {
            return Regex.Replace (text, @"[^\d]", "").ToCharArray ();
        }
    }
}
