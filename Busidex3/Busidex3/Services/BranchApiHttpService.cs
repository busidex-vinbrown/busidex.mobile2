using System;
using System.Net.Http;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class BranchApiHttpService : BaseHttpService
    {
        public static async Task<string> GetBranchUrl (QuickShareLink link)
        {            
            var model = new BranchApiLinkParameters {
                branch_key = ServiceResources.BRANCH_KEY,
                channel = "sms",
                feature = "share",
                campaign = "",
                tags = null,
                data = Newtonsoft.Json.JsonConvert.SerializeObject (new { cardId = link.CardId, _f = link.From, _d = link.DisplayName, _m = link.PersonalMessage})
            };

            var response = await MakeRequestAsync<BranchUrl>(StringResources.BRANCH_API_URL, HttpVerb.Post, model);

            return response.url;
        }
    }
}
