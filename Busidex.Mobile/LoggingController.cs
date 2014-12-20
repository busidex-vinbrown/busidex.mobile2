using System;

namespace Busidex.Mobile
{
	public class LoggingController : BaseController
	{
		public LoggingController ()
		{
		}

		public static void LogError(Exception ex, string userToken){

			const string URL = Resources.BASE_API_URL + "Error";
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
			MakeRequest (URL, "POST", userToken, data );
		}
	}
}

