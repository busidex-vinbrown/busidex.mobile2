using Busidex.Mobile.Models;


namespace Busidex.Mobile
{
	public class ActivityController : BaseController
	{

		public static void SaveActivity(long eventSourceId, long cardId, string userToken){
		
			var data = Newtonsoft.Json.JsonConvert.SerializeObject (new ActivityDTO {
				CardId = cardId,
				EventSourceId = eventSourceId,
				UserId = null
			});

			const string URL = Resources.BASE_API_URL + "Activity";

			MakeRequestAsync (URL, "POST", userToken, data,  new ModernHttpClient.NativeMessageHandler()).ContinueWith(r => {

			});
		}
	}
}

