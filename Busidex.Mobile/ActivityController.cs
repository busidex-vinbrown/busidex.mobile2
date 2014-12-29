
namespace Busidex.Mobile
{
	public class ActivityController : BaseController
	{

		public static void SaveActivity(long eventSourceId, long cardId, string userToken){
		
			string data = @"{'CardId':'" + cardId + "','UserId':null, 'EventSourceId'" + eventSourceId + "'}";

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Activity";

			MakeRequestAsync (url, "POST", userToken, data).ContinueWith(r => {

			});
		}
	}
}

