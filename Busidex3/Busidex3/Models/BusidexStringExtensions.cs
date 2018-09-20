using System.Text.RegularExpressions;

namespace Busidex3.Models
{
	public static class BusidexStringExtensions
	{
		public static string AsPhoneNumber (this string str)
		{
			if (string.IsNullOrEmpty (str)) {
				return str;
			}

			var digits = Regex.Replace (str, @"[^\d]", "").ToCharArray ();

			var display = string.Empty;
			for (var i = 0; i < digits.Length; i++) {
				switch (i) {
				case 0: {
						display += digits [i];
						break;
					}
				case 1: {
						display += digits [i];
						break;
					}
				case 2: {
						display += digits [i];
						break;
					}
				case 3: {
						display += "." + digits [i];
						break;
					}
				case 4: {
						display += digits [i];
						break;
					}
				case 5: {
						display += digits [i];
						break;
					}
				case 6: {
						display += "." + digits [i];
						break;
					}
				case 7: {
						display += digits [i];
						break;
					}
				case 8: {
						display += digits [i];
						break;
					}
				case 9: {
						display += digits [i];
						break;
					}
				}
			}
			return display;
		}
	}
}

