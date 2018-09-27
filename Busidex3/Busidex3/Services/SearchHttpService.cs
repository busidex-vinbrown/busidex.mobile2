using System.Collections.Generic;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class SearchHttpService : BaseHttpService
    {
        public async Task<SearchResultModel> DoSearch(string criteria, string userToken){

            var url = ServiceUrls.SearchUrl;

            var model = new SearchResultModel {
                CardType = CardType.Professional,
                Criteria = criteria,
                Display = 0,
                Distance = 0,
                HasResults = true,
                IsLoggedIn = true,
                Results = new List<CardDetailModel>(),
                SearchAddress = null,
                SearchLocation = 0,
                SearchText = criteria,
                TagCloud = null,
                UserId = null
            };

            return await MakeRequestAsync<SearchResultModel> (url, HttpVerb.Post, userToken, model);
        }

        public async Task<SearchResponse> SearchBySystemTag(string tag, string userToken)
        {
            var url = string.Format(ServiceUrls.SearchBySystemTagUrl, tag);
            return await MakeRequestAsync<SearchResponse> (url, HttpVerb.Post, userToken);
        }

        public async Task<EventListResponse> GetEventTags(string userToken)
        {
            var url = ServiceUrls.GetEventTagsUrl;
            return await MakeRequestAsync<EventListResponse> (url, HttpVerb.Get, userToken);
        }
    }
}
