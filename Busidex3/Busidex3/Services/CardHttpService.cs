﻿using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class CardHttpService : BaseHttpService
    {
        public static async Task<UserCard> GetCardById ( long cardId)
        {
            return await MakeRequestAsync<UserCard>(string.Format (ServiceUrls.CardDetailUrl, cardId), HttpVerb.Get);
        }

        public static async Task<UserCard> GetMyCard ()
        {
            return await MakeRequestAsync<UserCard> (ServiceUrls.MyCardUrl, HttpVerb.Get);
        }

        public static async Task<bool> UpdateCardImage (MobileCardImage card)
        {
            var result = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.SaveMobileCardImageUrl, HttpVerb.Put, data: card);
            return result.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateCardVisibility (byte visibility)
        {
            var url = string.Format(ServiceUrls.SaveCardVisibilityUrl, visibility);
            var result = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Put);
            return result.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateCardContactInfo (CardDetailModel card)
        {
            var result = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.SaveContactInfoUrl, HttpVerb.Put, card);
            return result.IsSuccessStatusCode;
        }
    }
}