using System.Net.Http;
using Busidex3.Models;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class ActivityHttpService : BaseHttpService
    {
        public static async void SaveActivity(long eventSourceId, long cardId, string userToken){
		
            var data = Newtonsoft.Json.JsonConvert.SerializeObject (new ActivityDTO {
                CardId = cardId,
                EventSourceId = eventSourceId,
                UserId = null
            });

            await MakeRequestAsync<HttpResponseMessage>(ServiceUrls.ActivityUrl, HttpVerb.Post, userToken, data);
        }
    }
}
