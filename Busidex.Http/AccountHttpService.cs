using System;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;

namespace Busidex.Http
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

        public async Task<UserAccount> CheckAccount(string email, string password, string displayName)
        {

            var data = new AutoResponseForm
            {
                uidId = Security.AuthToken ?? Guid.NewGuid().ToString(),
                email = email,
                pswd = password,
                dspname = displayName
            };

            return await MakeRequestAsync<UserAccount>(ServiceUrls.CheckAccountUrl, HttpVerb.Post, data);
        }

        public async Task<BusidexUser> GetAccount()
        {

            return await MakeRequestAsync<BusidexUser>(ServiceUrls.GetAccountUrl, HttpVerb.Get);
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            var result = await MakeRequestAsync<bool>(string.Format(ServiceUrls.CheckUserNameUrl, email), HttpVerb.Get);
            return result;
        }

        public async Task<UserAccount> UpdateDeviceType(DeviceType deviceType)
        {
            var url = ServiceUrls.UpdateDeviceTypeUrl + deviceType;
            return await MakeRequestAsync<UserAccount>(url, HttpVerb.Put);
        }

        public async Task<UpdateUserResponse> UpdateUser(UserDTO userDto)
        {
            var url = ServiceUrls.UpdateUserUrl;
            return await MakeRequestAsync<UpdateUserResponse>(url, HttpVerb.Put, userDto);
        }
    }
}
