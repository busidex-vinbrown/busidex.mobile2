using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class AccountHttpService : BaseHttpService
    {
        public async Task<UserAccount> UpdateDisplayNameAsync(string name)
        {
            var encodedName = System.Net.WebUtility.HtmlEncode(name);
            var data = @"{'name':'" + name + "'}";

            var url = ServiceUrls.UpdateDisplayNameUrl + encodedName;

            return await MakeRequestAsync<UserAccount>(url, HttpVerb.Put, data);
        }

        public async Task<UserAccount> CheckAccount(string email, string password)
        {

            var data = new AutoResponseForm
            {
                uidId = Security.AuthToken,
                email = email,
                pswd = password
            };

            return await MakeRequestAsync<UserAccount>(ServiceUrls.CheckAccountUrl, HttpVerb.Post, data);
        }

        public async Task<BusidexUser> GetAccount()
        {

            return await MakeRequestAsync<BusidexUser>(ServiceUrls.GetAccountUrl, HttpVerb.Get);
        }

        public async Task<UserAccount> UpdateDeviceType(string token, DeviceType deviceType)
        {

            var url = ServiceUrls.UpdateDeviceTypeUrl + deviceType;
            return await MakeRequestAsync<UserAccount>(url, HttpVerb.Put, token);
        }
    }
}
