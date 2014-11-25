using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class OrganizationController : BaseController
	{
	
		public OrganizationController ()
		{
		}

		public async Task<string> GetMyOrganizations(string userToken){

			const string url = Busidex.Mobile.Resources.BASE_API_URL + "Organization";
			return await MakeRequest (url, "GET", userToken);
		}

		public async Task<string> GetOrganizationById(string userToken, long id){

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Organization?id=" + id;
			return await MakeRequest (url, "GET", userToken);
		}

		public async Task<string> GetOrganizationMembers(string userToken, long id){

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Organization/GetMembers?organizationId=" + id;
			return await MakeRequest (url, "GET", userToken);
		}

		public async Task<string> GetOrganizationReferrals(string userToken, long id){

			string url = Busidex.Mobile.Resources.BASE_API_URL + "Organization/GetReferrals?organizationId=" + id;
			return await MakeRequest (url, "GET", userToken);
		}
	}
}

