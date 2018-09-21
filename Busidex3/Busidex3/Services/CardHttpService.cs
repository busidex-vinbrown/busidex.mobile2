using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class CardHttpService : BaseHttpService
    {
        public static async Task<UserCard> GetCardById (string userToken, long cardId)
        {
            return await MakeRequestAsync<UserCard>(string.Format (ServiceUrls.CardDetailUrl, cardId), HttpVerb.Get, userToken);
        }

        public static async Task<UserCard> GetMyCard ()
        {
            var authToken = ""; //UISubscriptionService.AuthToken
            return await MakeRequestAsync<UserCard> (ServiceUrls.MyCardUrl, HttpVerb.Get, authToken);
        }

        public static async Task<bool> UpdateCardImage (MobileCardImage card)
        {
            var authToken = ""; //UISubscriptionService.AuthToken
            var result = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.SaveMobileCardImageUrl, HttpVerb.Put, authToken, data: card);
            return result.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateCardVisibility (byte visibility)
        {
            var url = string.Format(ServiceUrls.SaveCardVisibilityUrl, visibility);
            var authToken = ""; //UISubscriptionService.AuthToken
            var result = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Put, authToken);
            return result.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateCardContactInfo (CardDetailModel card)
        {
            var authToken = ""; //UISubscriptionService.AuthToken
            var result = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.SaveContactInfoUrl, HttpVerb.Put, authToken, card);
            return result.IsSuccessStatusCode;
        }
    }
}
