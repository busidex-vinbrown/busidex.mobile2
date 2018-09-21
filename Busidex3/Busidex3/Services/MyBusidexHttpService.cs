using System.Net.Http;
using Busidex3.DomainModels;
using System.Threading.Tasks;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class MyBusidexHttpService : BaseHttpService
    {
        public async Task<MyBusidexResponse> GetMyBusidex(string userToken)
        {
            return await MakeRequestAsync<MyBusidexResponse>(ServiceUrls.MyBusidexUrl, HttpVerb.Get, userToken);
        }

        public async Task<bool> AddToMyBusidex(long cardId, string userToken){
            var url = string.Format(ServiceUrls.AddToMyBusidexUrl, cardId);

            var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Post, userToken);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromMyBusidex(long cardId, string userToken){
            var url = string.Format(ServiceUrls.RemoveFromMyBusidexUrl, cardId);

            var resp = await MakeRequestAsync<HttpResponseMessage> (url, HttpVerb.Delete, userToken);
            return resp.IsSuccessStatusCode;
        }
    }
}
