using System.Net.Http;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;

namespace Busidex.Http
{
    public class CardHttpService : BaseHttpService
    {
        public async Task<CardDetailResponse> GetCardById(long cardId)
        {
            return await MakeRequestAsync<CardDetailResponse>(string.Format(ServiceUrls.CardDetailUrl, cardId), HttpVerb.Get);
        }

        public async Task<CardDetailResponse> GetMyCard()
        {
            return await MakeRequestAsync<CardDetailResponse>(ServiceUrls.MyCardUrl, HttpVerb.Get);
        }

        public async Task<bool> UpdateCardImage(MobileCardImage card)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveMobileCardImageUrl, HttpVerb.Put,
                    data: card);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardVisibility(byte visibility)
        {
            var url = string.Format(ServiceUrls.SaveCardVisibilityUrl, visibility);
            var result = await MakeRequestAsync<HttpResponseMessage>(url, HttpVerb.Put);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardSearchInfo(string json)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveSearchInfoUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardContactInfo(string json)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveContactInfoUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardAddress(string json)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveCardAddressUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardTags(string json)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveTagsUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardLinks(string json)
        {            
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveExternalLinksUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardOwner(string json)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.UpdateOwnerIdUrl, HttpVerb.Put, json);
            return result.IsSuccessStatusCode;
        }
    }
}
