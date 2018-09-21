using System.Collections.Generic;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;

namespace Busidex3.Services
{
    public class OrganizationsHttpService : BaseHttpService
    {
        public async Task<OrganizationResponse> GetMyOrganizations(string userToken)
        {
            var url = ServiceUrls.OrganizationListUrl;
            return await MakeRequestAsync<OrganizationResponse> (url, HttpVerb.Get, userToken);
        }

        public async Task<Organization> GetOrganizationById(string userToken, long id){

            var url = string.Format(ServiceUrls.OrganizationUrl, id);
            return await MakeRequestAsync<Organization> (url, HttpVerb.Get, userToken);
        }

        public async Task<OrgMemberResponse> GetOrganizationMembers(string userToken, long id){

            var url = string.Format(ServiceUrls.OrganizationMembersUrl, id);
            return await MakeRequestAsync<OrgMemberResponse> (url, HttpVerb.Get, userToken);
        }

        public async Task<OrgReferralResponse> GetOrganizationReferrals(string userToken, long id){

            var url = string.Format(ServiceUrls.OrganizationReferralsUrl, id);
            return await MakeRequestAsync<OrgReferralResponse> (url, HttpVerb.Get, userToken);
        }
    }
}
