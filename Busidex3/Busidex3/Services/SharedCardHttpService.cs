using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class SharedCardHttpService : BaseHttpService
    {
        public static async Task<bool> ShareCard(Card card, string email, string message)
        {
            var url = ServiceUrls.ShareCardUrl;
			var model = new List<SharedCard> () {
				new SharedCard {
					SharedCardId = 0,
					CardId = card.CardId,
					SendFrom = 0,
					SendFromEmail = string.Empty,
					Email = email,
					PhoneNumber = string.Empty,
					ShareWith = 0,
					SharedDate = DateTime.Now,
					Accepted = false,
					Declined = false,
					Recommendation = message,
					UseQuickShare = false
				}
			};
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Post, model);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<List<SharedCard>> GetSharedCards()
        {
            return await MakeRequestAsync<List<SharedCard>>(ServiceUrls.GetSharedCardUrl, HttpVerb.Get);
        }

        public static async Task<bool> UpdateSharedCards(long? acceptedCardId, long? declinedCardId){

			var model = new SharedCardModel {
				AcceptedCardIdList = acceptedCardId.HasValue ? new[]{ acceptedCardId.Value } : new long[]{},
				DeclinedCardIdList = declinedCardId.HasValue ? new[]{ declinedCardId.Value } : new long[]{},
				CardIdList = null,
				SharedWith = string.Empty,
				Accepted = false,
				Declined = false,
				UserId = 0,
				PersonalMessage = string.Empty
			};
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.UpdateSharedCardUrl, HttpVerb.Put, data);
            return resp.IsSuccessStatusCode;
        }

		public async Task<bool> AcceptQuickShare(Card card, string email, long sendFrom, string message){

			var model = new SharedCard {
				SharedCardId = 0,
				CardId = card.CardId,
				SendFrom = sendFrom,
				SendFromEmail = string.Empty,
				Email = email,
				PhoneNumber = string.Empty,
				ShareWith = Security.DecodeUserId (), // share with the current user
				SharedDate = DateTime.Now,
				Accepted = true,
				Declined = false,
				Recommendation = message,
				UseQuickShare = true
			};
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);

			var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.AcceptQuickShareUrl, HttpVerb.Post, data);
		    return resp.IsSuccessStatusCode;
		}
    }
}
