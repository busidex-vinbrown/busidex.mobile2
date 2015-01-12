using System;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class SharedCardController : BaseController
	{
		/// <summary>
		/// POST
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="card">Card.</param>
		/// <param name="email">Email.</param>
		/// <param name="userToken">User token.</param>
		public string ShareCard(Card card, string email, string userToken){
			const string URL = Resources.BASE_API_URL + "SharedCard/Post";
//			var model = new List<SharedCard> () {
//				new SharedCard {
//					SharedCardId = 0,
//					CardId = card.CardId,
//					SendFrom = 0,
//					SendFromEmail = string.Empty,
//					Email = email,
//					ShareWith = 0,
//					SharedDate = DateTime.Now,
//					Accepted = false,
//					Declined = false,
//					Recommendation = string.Empty
//				}
//			};
//			var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var data =  string.Format(
				"'SharedCardId': 0," +
				"'CardId': {0}," +
				"'SendFrom': 0," +
				"'SendFromEmail':''," +
				"'Email': '{1}', " +
				"'ShareWith': 0, " +
				"'SharedDate': '{2}'," +
				"'Accepted': 'false',	" +
				"'Declined' :'false', " +
				"'Recommendation': ''", 
				card.CardId, email, DateTime.Now);

			data = "[{" + data + "}]";
			return MakeRequest (URL, "POST", userToken, data);
		}

		/// <summary>
		/// GET
		/// </summary>
		/// <returns>The shared cards.</returns>
		/// <param name="userToken">User token.</param>
		public string GetSharedCards(string userToken){

			const string URL = Resources.BASE_API_URL + "SharedCard/Get";
			return MakeRequest (URL, "GET", userToken);
		}

		/// <summary>
		/// PUT
		/// </summary>
		/// <returns>The shared cards.</returns>
		/// <param name="acceptedCardId">Accepted card identifier.</param>
		/// <param name="declinedCardId">Declined card identifier.</param>
		/// <param name="userToken">User token.</param>
		public string UpdateSharedCards(long? acceptedCardId, long? declinedCardId, string userToken){
			const string URL = Resources.BASE_API_URL + "SharedCard/Put";

//			var model = new SharedCardModel {
//				AcceptedCardIdList = acceptedCardId.HasValue ? new long[]{ acceptedCardId.Value } : new long[]{},
//				DeclinedCardIdList = declinedCardId.HasValue ? new long[]{ declinedCardId.Value } : new long[]{},
//				CardIdList = null,
//				SharedWith = string.Empty,
//				Accepted = false,
//				Declined = false,
//				UserId = 0,
//				PersonalMessage = string.Empty
//			};
			var data =  string.Format(
				"'UserId': 0," +
				"'AcceptedCardIdList': [{0}]," +
				"'DeclinedCardIdList': [{1}]," +
				"'CardIdList': []," +
				"'SharedWith': '', " +
				"'Accepted': 'false', " +
				"'Declined': 'false', " +
				"'PersonalMessage': ''", 
				acceptedCardId.HasValue ? acceptedCardId.Value.ToString() : null, 
				declinedCardId.HasValue ? declinedCardId.Value.ToString() : null);
			data = "{" + data + "}";
			//var data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			return MakeRequest (URL, "PUT", userToken, data);
		}
	}
}