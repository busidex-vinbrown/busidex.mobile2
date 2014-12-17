using System;
using System.Threading.Tasks;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class SharedCardController : BaseController
	{

		public Task<string> ShareCard(Card card, string email, string userToken){
			const string URL = Resources.BASE_API_URL + "SharedCard";
			var model = new SharedCard {
				SharedCardId = 0,
				CardId = card.CardId,
				SendFrom = 0,
				SendFromEmail =  string.Empty,
				Email = email,
				ShareWith = null,
				SharedDate = DateTime.Now,
				Accepted = null,
				Declined = null,
				Recommendation = string.Empty,
				Card = card
			};
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);

			return MakeRequestAsync (URL, "POST", userToken, data);
		}
	}
}

