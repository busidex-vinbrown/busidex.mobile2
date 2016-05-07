using System.Threading.Tasks;


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
	}
}

