using System.Threading.Tasks;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class SMSShareController : BaseController
	{
		public static async Task<bool> SaveSmsShare (long fromId, long cardId, string phoneNumber, string message, string token)
		{

			var data = /*Newtonsoft.Json.JsonConvert.SerializeObject (*/new SMSShare {
				FromUserId = fromId,
				CardId = cardId,
				PhoneNumber = phoneNumber,
				Message = message
			};//);

			const string URL = Resources.BASE_API_URL + "SmSShare/Post";

			await MakeRequestAsync (URL, "POST", token, data, new ModernHttpClient.NativeMessageHandler ());

			return true;
		}
	}
}

