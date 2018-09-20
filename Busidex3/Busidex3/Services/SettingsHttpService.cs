using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class SettingsHttpService : BaseHttpService
    {
        public static async Task<bool> ChangeUserName(string name, string userToken){

            var url = string.Format(ServiceUrls.ChangeUserNameUrl, name);
            var data = "{}";
            var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Put, userToken, data);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<bool> ChangePassword(string oldPassword, string newPassword, string userToken){
		
            var data = @"{'UserId':0, 'OldPassword':'" + oldPassword + "','NewPassword':'" + newPassword + "','ConfirmPassword':'" + newPassword + "'}";

            var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.ChangePasswordUrl, HttpVerb.Put, userToken, data);
            return resp.IsSuccessStatusCode;
        }

        public static async Task<bool> ChangeEmail(string email, string userToken){

            var url = string.Format(ServiceUrls.ChangeEmailUrl, email);
            var data = "{}";
            var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Put, userToken, data);
            return resp.IsSuccessStatusCode;
        }
    }
}
