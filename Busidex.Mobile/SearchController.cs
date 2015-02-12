using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class SearchController : BaseController
	{
		public async Task<string> DoSearch(string criteria, string userToken){

			const string url = Resources.BASE_API_URL + "search/Search";
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

			return await MakeRequestAsync (url, "POST", userToken, data);
		}

		public async Task<string> SearchBySystemTag(string tag, string userToken){
			string url = Resources.BASE_API_URL + "search/SystemTagSearch?systag=" + tag;
			return await MakeRequestAsync (url, "GET", userToken);
		}

		public string GetEventTags(string tag, string userToken){
			const string url = Resources.BASE_API_URL + "search/GetEventTags";
			return MakeRequest (url, "GET", userToken);
		}
	}
}