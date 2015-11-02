using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class OrganizationController : BaseController
	{
	
		public async Task<string> GetMyOrganizations(string userToken){

			const string url = Resources.BASE_API_URL + "Organization";
			return await MakeRequestAsync (url, "GET", userToken, null, new ModernHttpClient.NativeMessageHandler());
		}

		public async Task<string> GetOrganizationById(string userToken, long id){

			string url = Resources.BASE_API_URL + "Organization?id=" + id;
			return await MakeRequestAsync (url, "GET", userToken, null,  new ModernHttpClient.NativeMessageHandler());
		}

		public async Task<string> GetOrganizationMembers(string userToken, long id){

			string url = Resources.BASE_API_URL + "Organization/GetMembers?organizationId=" + id;
			return await MakeRequestAsync (url, "GET", userToken,null,  new ModernHttpClient.NativeMessageHandler());
		}

		public async Task<string> GetOrganizationReferrals(string userToken, long id){

			string url = Resources.BASE_API_URL + "Organization/GetReferrals?organizationId=" + id;
			return await MakeRequestAsync (url, "GET", userToken,null,  new ModernHttpClient.NativeMessageHandler());
		}
	}
}

