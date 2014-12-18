using System;
using System.Threading.Tasks;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Mobile
{
	public class SharedCardController : BaseController
	{

		public string ShareCard(Card card, string email, string userToken){
			const string URL = Resources.BASE_API_URL + "SharedCard/Post";
			var model = new List<SharedCard> () {
				new SharedCard {
					SharedCardId = 0,
					CardId = card.CardId,
					SendFrom = 0,
					SendFromEmail = string.Empty,
					Email = email,
					ShareWith = 0,
					SharedDate = DateTime.Now,
					Accepted = false,
					Declined = false,
					Recommendation = string.Empty
				}
			};
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);

			return MakeRequest (URL, "POST", userToken, data);
		}
	}
}

