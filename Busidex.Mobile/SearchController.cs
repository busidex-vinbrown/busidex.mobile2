using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class SearchController
	{
		public SearchController ()
		{
		}

		public async Task<string> DoSearch(string criteria, string userToken){

			string url = "https://www.busidexapi.com/api/search/Search";
			string data = 
			@"{" + 
				"'Success': true," + 
				"'SearchModel': {" + 
				"'UserId': 0," + 
			"'Criteria': '" + criteria + "'," + 
			"'SearchText': '" + criteria + "'," + 
			"'SearchAddress': null," + 
			"'SearchLocation': 0," + 
			"'Results': []," + 
			"'IsLoggedIn': true," + 
			"'HasResults': true," + 
			"'Display': 0," + 
			"'Distance': 25," + 
			"'TagCloud': { }," + 
			"'CardType': 0" + 
			"}," + 
			"'TagSearch': false," + 
			"'SearchText': '" + criteria + "'," + 
			"'NoResults': false," + 
			"'SearchResultsMessage': ''," + 
			"'UserId': 0," + 
			"'CardType': 1" + 
			"}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";

			using (var writer = new StreamWriter (request.GetRequestStream ())) {
				writer.Write (data);
			}

			request.Headers.Add ("X-Authorization-Token", userToken);

			try {
				WebResponse webResponse = await request.GetResponseAsync();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();

				responseReader.Close();

				return response;

			} catch (Exception e) {
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}
			return string.Empty;
		}
	}
}