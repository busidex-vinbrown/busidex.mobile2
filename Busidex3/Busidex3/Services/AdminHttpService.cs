using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using System.Threading.Tasks;

namespace Busidex3.Services
{
    public class AdminHttpService : BaseHttpService
    {
        public async Task<UnownedCardResponse> GetUnownedCards()
        {
            return await MakeRequestAsync<UnownedCardResponse>(ServiceUrls.GetUnownedCardsUrl, HttpVerb.Get);
        }

        public async Task<OwnerEmailResponse> SendOwnerEmails(long cardId, string email)
        {
            var url = string.Format(ServiceUrls.SendOwnerEmailsUrl, cardId, email);
            return await MakeRequestAsync<OwnerEmailResponse>(url, HttpVerb.Post);
        }
    }
}
