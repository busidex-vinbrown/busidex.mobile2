using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Dto;

namespace Busidex.Http
{
    public class SettingsHttpService : BaseHttpService
    {
        public static async Task<bool> ChangeUserName(string name)
        {

            var url = string.Format(ServiceUrls.ChangeUserNameUrl, name);
            var data = "{}";
            var resp = await MakeRequestAsync<HttpResponseMessage>(url, HttpVerb.Put, data);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {

            var data = @"{'UserId':0, 'OldPassword':'" + oldPassword + "','NewPassword':'" + newPassword + "','ConfirmPassword':'" + newPassword + "'}";

            var resp = await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.ChangePasswordUrl, HttpVerb.Put, data);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<bool> ChangeEmail(string email)
        {

            var url = string.Format(ServiceUrls.ChangeEmailUrl, email);
            var data = "{}";
            var resp = await MakeRequestAsync<HttpResponseMessage>(url, HttpVerb.Put, data);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<List<SystemSettingDto>> GetSystemSettings()
        {
            var url = ServiceUrls.SystemSettingsUrl;
            var resp = await MakeRequestAsync<SystemSettingsResponse>(url, HttpVerb.Get);
            var settings = resp.Model.Result;
            return settings;
        }
    }
}
