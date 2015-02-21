using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class AccountController : BaseController
	{
		const string UPDATE_DISPLAY_NAME_URL = "Account/UpdateDisplayName?name=";
		const string CHECK_ACCOUNT_URL = Resources.BASE_API_URL + "Registration/CheckAccount";
		const string GET_ACCOUNT_URL = Resources.BASE_API_URL + "Account/Get?id=0";

		public static async Task<string> UpdateDisplayNameAsync(string name, string userToken){
			string encodedName = System.Net.WebUtility.HtmlEncode (name);
			string data = @"{'name':'" + name + "'}";

			string url = Resources.BASE_API_URL + UPDATE_DISPLAY_NAME_URL + encodedName;

			return await MakeRequestAsync (url, Resources.HttpActions.PUT.ToString(), userToken, data);
		}

		public static string UpdateDisplayName(string name, string userToken){
			string encodedName = System.Net.WebUtility.HtmlEncode (name);
			string data = @"{'name':'" + name + "'}";

			string url = Resources.BASE_API_URL + UPDATE_DISPLAY_NAME_URL + encodedName;

			return MakeRequest (url, Resources.HttpActions.PUT.ToString(), userToken, data);
		}

		public static string CheckAccount(string token, string email, string password){
			string data = @"{'uidId':'" + token + "','email':'" + email + "', 'pswd':'" + password + "'}";

			return MakeRequest (CHECK_ACCOUNT_URL, Resources.HttpActions.POST.ToString(), token, data);
		}

		public static string GetAccount(string token){

			return MakeRequest (GET_ACCOUNT_URL, Resources.HttpActions.GET.ToString(), token);
		}
	}
}

