using System.Threading.Tasks;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class CardController : BaseController
	{
		public static string GetCardById (string userToken, long cardId)
		{
			const string url = Resources.BASE_API_URL + "card/details/{0}";
			return MakeRequest (string.Format (url, cardId), "GET", userToken);
		}

		public static async Task<string> GetMyCard ()
		{
			const string url = Resources.BASE_API_URL + "card/Get";
			return await MakeRequestAsync (url, "GET", UISubscriptionService.AuthToken, handler: new ModernHttpClient.NativeMessageHandler ());
		}

		public static async Task<string> UpdateCardImage (MobileCardImage card)
		{
			const string url = Resources.BASE_API_URL + "card/SaveMobileCardImage";
			return await MakeRequestAsync (url, "PUT", UISubscriptionService.AuthToken, data: card, handler: new ModernHttpClient.NativeMessageHandler ());
		}

		public static async Task<string> UpdateCardVisibility (byte visibility)
		{
			string url = Resources.BASE_API_URL + "card/SaveCardVisibility?visibility=" + visibility;
			return await MakeRequestAsync (url, "PUT", UISubscriptionService.AuthToken, handler: new ModernHttpClient.NativeMessageHandler ());
		}

		public static async Task<string> UpdateCardContactInfo (CardDetailModel card)
		{
			string url = Resources.BASE_API_URL + "card/SaveContactInfo";
			return await MakeRequestAsync (url, "PUT", UISubscriptionService.AuthToken, data: card, handler: new ModernHttpClient.NativeMessageHandler ());
		}
	}
}

