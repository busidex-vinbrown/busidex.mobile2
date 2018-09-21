using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class AccountHttpService : BaseHttpService
    {
        public static async Task<UserAccount> UpdateDisplayNameAsync(string name, string userToken){
            var encodedName = System.Net.WebUtility.HtmlEncode (name);
            var data = @"{'name':'" + name + "'}";

            var url = ServiceUrls.UpdateDisplayNameUrl + encodedName;

            return await MakeRequestAsync<UserAccount> (url, HttpVerb.Put, userToken, data);
        }
 
        public static async Task<UserAccount> CheckAccount(string token, string email, string password){

            var data = new AutoResponseForm{
                uidId = token,
                email = email,
                pswd = password
            };

            return await MakeRequestAsync<UserAccount> (ServiceUrls.CheckAccountUrl, HttpVerb.Post, token, data);
        }

        public static async Task<UserAccount> GetAccount(string token){

            return await MakeRequestAsync<UserAccount>(ServiceUrls.GetAccountUrl, HttpVerb.Get, token);
        }

        public static async Task<UserAccount> UpdateDeviceType(string token, DeviceType deviceType){

            var url = ServiceUrls.UpdateDeviceTypeUrl + deviceType;
            return await MakeRequestAsync<UserAccount> (url, HttpVerb.Put, token);
        }
    }
}
