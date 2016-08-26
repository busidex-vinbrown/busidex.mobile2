using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class UserDeviceController : BaseController
	{
		public static async Task<bool> UpdateDeviceDetails (DeviceType deviceType, int version, string token)
		{

			var data = new UserDevice {
				UserId = 0,
				DeviceTypeId = (int)deviceType,
				Version = version
			};

			const string URL = Resources.BASE_API_URL + "UserDevice/UpdateUserDevice";

			await MakeRequestAsync (URL, "PUT", token, data, new ModernHttpClient.NativeMessageHandler ());

			return true;
		}

		public static async Task<UserDevice> GetUserDevice (DeviceType deviceType, string token)
		{
			string URL = Resources.BASE_API_URL + "UserDevice/GetDeviceDetails?deviceType=" + deviceType;

			var result = await MakeRequestAsync (URL, "GET", token, null, new ModernHttpClient.NativeMessageHandler ());
			var userDevice = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDevice> (result);
			return userDevice;
		}

		public static async Task<AppVersionInfo> GetCurrentAppInfo (string token)
		{
			string URL = Resources.BASE_API_URL + "UserDevice/GetCurrentAppInfo";

			var result = await MakeRequestAsync (URL, "GET", token, null, new ModernHttpClient.NativeMessageHandler ());
			var appInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AppVersionInfo> (result);
			return appInfo;
		}
	}
}

