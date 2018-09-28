using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class UserDeviceHttpService : BaseHttpService
    {
        public static async Task<bool> UpdateDeviceDetails (DeviceType deviceType, int version)
        {
            var data = new UserDevice {
                UserId = 0,
                DeviceTypeId = (int)deviceType,
                Version = version
            };

            var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.UpdateUserDeviceUrl, HttpVerb.Put, data);

            return resp.IsSuccessStatusCode;
        }

        public static async Task<UserDevice> GetUserDevice (DeviceType deviceType)
        {
            var url = string.Format(ServiceUrls.DeviceDetailsUrl, deviceType);

            return await MakeRequestAsync<UserDevice>(url, HttpVerb.Get);
        }

        public static async Task<AppVersionInfo> GetCurrentAppInfo ()
        {
            return await MakeRequestAsync<AppVersionInfo> (ServiceUrls.AppInfoUrl, HttpVerb.Get);
        }
    }
}
