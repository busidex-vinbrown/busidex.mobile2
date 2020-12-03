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

        public async Task<bool> UpdateCardContactInfo(CardDetailModel card)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveContactInfoUrl, HttpVerb.Put, card);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardLinks(CardLinksModel model)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.SaveExternalLinksUrl, HttpVerb.Put, model);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCardOwner(long cardId, long ownerId)
        {
            var result =
                await MakeRequestAsync<HttpResponseMessage>(string.Format(ServiceUrls.UpdateOwnerIdUrl, cardId, ownerId), HttpVerb.Put);
            return result.IsSuccessStatusCode;
        }
    }
}
