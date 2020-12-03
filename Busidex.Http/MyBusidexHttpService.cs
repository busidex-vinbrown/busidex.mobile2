using System.Net.Http;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;

namespace Busidex.Http
{
    public class MyBusidexHttpService : BaseHttpService
    {
        public async Task<MyBusidexResponse> GetMyBusidex()
        {
            return await MakeRequestAsync<MyBusidexResponse>(ServiceUrls.MyBusidexUrl, HttpVerb.Get);
        }

        public async Task<bool> AddToMyBusidex(long cardId)
        {
            var url = string.Format(ServiceUrls.AddToMyBusidexUrl, cardId);

            var resp = await MakeRequestAsync<HttpResponseMessage>(url, HttpVerb.Post);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromMyBusidex(long cardId)
        {
            var url = string.Format(ServiceUrls.RemoveFromMyBusidexUrl, cardId);

            var resp = await MakeRequestAsync<HttpResponseMessage>(url, HttpVerb.Delete);
            return resp.IsSuccessStatusCode;
        }
    }
}
