using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class UrlShortenerController : BaseController
	{
		const string API_URL = "https://api-ssl.bitly.com/v3/shorten?access_token={0}&longUrl={1}&format=txt";

		public static async Task<string> ShortenUrl(string url){

			string request = string.Format (API_URL, Resources.BITLY_ACCESS_TOKEN, url);
			return await MakeExternalReequest (request, "GET", request);
		}
	}
}

