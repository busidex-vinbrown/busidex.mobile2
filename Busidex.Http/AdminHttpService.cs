using Busidex.Http.Utils;
using Busidex.Models.Dto;
using System.Threading.Tasks;

namespace Busidex.Http
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
