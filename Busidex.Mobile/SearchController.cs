using System.Threading.Tasks;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class SearchController : BaseController
	{
		public async Task<string> DoSearch(string criteria, string userToken){

			const string url = Resources.BASE_API_URL + "search/Search";
//			string data = 
//			@"{" + 
//				"'Success': true," + 
//				"'SearchModel': {" + 
//				"'UserId': 0," + 
//			"'Criteria': '" + criteria + "'," + 
//			"'SearchText': '" + criteria + "'," + 
//			"'SearchAddress': null," + 
//			"'SearchLocation': 0," + 
//			"'Results': []," + 
//			"'IsLoggedIn': true," + 
//			"'HasResults': true," + 
//			"'Display': 0," + 
//			"'Distance': 25," + 
//			"'TagCloud': { }," + 
//			"'CardType': 0" + 
//			"}," + 
//			"'TagSearch': false," + 
//			"'SearchText': '" + criteria + "'," + 
//			"'NoResults': false," + 
//			"'SearchResultsMessage': ''," + 
//			"'UserId': 0," + 
//			"'CardType': 1" + 
//			"}";

			var model = new SearchResultModel {
				CardType = CardType.Professional,
				Criteria = criteria,
				Display = 0,
				Distance = 0,
				HasResults = true,
				IsLoggedIn = true,
				Results = new System.Collections.Generic.List<CardDetailModel>(),
				SearchAddress = null,
				SearchLocation = 0,
				SearchText = criteria,
				TagCloud = null,
				UserId = null
			};
			//data = Newtonsoft.Json.JsonConvert.SerializeObject (model);

			return await MakeRequestAsync (url, "POST", userToken, model, new ModernHttpClient.NativeMessageHandler());
		}

		public async Task<string> SearchBySystemTag(string tag, string userToken){
			string url = Resources.BASE_API_URL + "search/SystemTagSearch?systag=" + tag + "&ownedOnly=true";
			return await MakeRequestAsync (url, "POST", userToken, null,  new ModernHttpClient.NativeMessageHandler());
		}

		public async Task<string> GetEventTags(string userToken){
			const string url = Resources.BASE_API_URL + "search/GetEventTags";
			return await MakeRequestAsync (url, "GET", userToken, null,  new ModernHttpClient.NativeMessageHandler());
		}
	}
}