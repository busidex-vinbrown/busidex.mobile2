﻿using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class UserDeviceHttpService : BaseHttpService
    {
        public static async Task<bool> UpdateDeviceDetails (DeviceType deviceType, int version, string token)
        {
            var data = new UserDevice {
                UserId = 0,
                DeviceTypeId = (int)deviceType,
                Version = version
            };

            var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.UpdateUserDeviceUrl, HttpVerb.Put, token, data);

            return resp.IsSuccessStatusCode;
        }

        public static async Task<UserDevice> GetUserDevice (DeviceType deviceType, string token)
        {
            var url = string.Format(ServiceUrls.DeviceDetailsUrl, deviceType);

            return await MakeRequestAsync<UserDevice>(url, HttpVerb.Get, token);
        }

        public static async Task<AppVersionInfo> GetCurrentAppInfo (string token)
        {
            return await MakeRequestAsync<AppVersionInfo> (ServiceUrls.AppInfoUrl, HttpVerb.Get, token);
        }
    }
}
