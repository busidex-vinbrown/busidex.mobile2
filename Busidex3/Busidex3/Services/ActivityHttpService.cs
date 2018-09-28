using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class ActivityHttpService : BaseHttpService
    {
        public async Task<bool> SaveActivity(long eventSourceId, long cardId){
		
            var data = Newtonsoft.Json.JsonConvert.SerializeObject (new ActivityDTO {
                CardId = cardId,
                EventSourceId = eventSourceId,
                UserId = null
            });

            var response = await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.ActivityUrl, HttpVerb.Post, data);
            return response.IsSuccessStatusCode;
        }
    }
}
