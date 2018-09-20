using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.Models;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class SMSShareHttpService : BaseHttpService
    {
        public static async Task<bool> SaveSmsShare (long fromId, long cardId, string phoneNumber, string message, string token)
        {

            var data = new SMSShare {
                FromUserId = fromId,
                CardId = cardId,
                PhoneNumber = phoneNumber,
                Message = message
            };

            var resp = await MakeRequestAsync<HttpResponseMessage> (ServiceUrls.SmsShareUrl, HttpVerb.Post, token, data);

            return resp.IsSuccessStatusCode;
        }
    }
}
