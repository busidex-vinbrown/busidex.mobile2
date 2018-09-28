using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class OrganizationsHttpService : BaseHttpService
    {
        public async Task<OrganizationResponse> GetMyOrganizations()
        {
            var url = ServiceUrls.OrganizationListUrl;
            return await MakeRequestAsync<OrganizationResponse> (url, HttpVerb.Get);
        }

        public async Task<Organization> GetOrganizationById(long id){

            var url = string.Format(ServiceUrls.OrganizationUrl, id);
            return await MakeRequestAsync<Organization> (url, HttpVerb.Get);
        }

        public async Task<OrgMemberResponse> GetOrganizationMembers(long id){

            var url = string.Format(ServiceUrls.OrganizationMembersUrl, id);
            return await MakeRequestAsync<OrgMemberResponse> (url, HttpVerb.Get);
        }

        public async Task<OrgReferralResponse> GetOrganizationReferrals(long id){

            var url = string.Format(ServiceUrls.OrganizationReferralsUrl, id);
            return await MakeRequestAsync<OrgReferralResponse> (url, HttpVerb.Get);
        }
    }
}
