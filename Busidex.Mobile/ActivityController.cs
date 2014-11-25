using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class ActivityController : BaseController
	{
		public ActivityController ()
		{
		}

		public static async Task<string> SaveActivity(long eventSourceId, long cardId, string userToken){
		
			string data = @"{'CardId':'" + cardId + "','UserId':null, 'EventSourceId'" + eventSourceId + "'}";

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Activity";

			return await MakeRequest (url, "POST", userToken, data);
		}
	}
}

