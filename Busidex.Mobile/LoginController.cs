
namespace Busidex.Mobile
{
	public class LoginController: BaseController
	{
		const string LOGIN_URL = "Account/Login";


		public static string DoLogin(string username, string password){
		
			string data = "{'UserName':'" + username + "','Password':'" + password + "','Token':'','RememberMe':'true'}";
			string url = string.Format ("{0}{1}", Resources.BASE_API_URL, LOGIN_URL);
			return MakeRequest (url, "POST", string.Empty, data);
		}
	}
}

