﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;

namespace Busidex.Http
{
    public class SearchHttpService : BaseHttpService
    {
        public async Task<SearchResponse> DoSearch(string criteria)
        {

            var url = ServiceUrls.SearchUrl;

            var model = new SearchResultModel
            {
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

            return await MakeRequestAsync<SearchResponse>(url, HttpVerb.Post, model);
        }

        public async Task<SearchResponse> SearchBySystemTag(string tag)
        {
            var url = string.Format(ServiceUrls.SearchBySystemTagUrl, tag);
            return await MakeRequestAsync<SearchResponse>(url, HttpVerb.Post);
        }

        public async Task<EventListResponse> GetEventTags()
        {
            var url = ServiceUrls.GetEventTagsUrl;
            return await MakeRequestAsync<EventListResponse>(url, HttpVerb.Get);
        }

        public async Task<EventListResponse> GetUserEventTags()
        {
            var url = ServiceUrls.GetUserEventTagsUrl;
            return await MakeRequestAsync<EventListResponse>(url, HttpVerb.Get);
        }
    }
}