using System.Threading.Tasks;


namespace Busidex.Mobile
{
	public class LoginController: BaseController
	{
		const string LOGIN_URL = "Account/Login";


		public async Task<string> DoLogin(string username, string password){
		
			//string data = "{'UserName':'" + username + "','Password':'" + password + "','Token':'','RememberMe':'true'}";

			var model = new LoginParams {
				UserName = username,
				Password = password, 
				Token = string.Empty,
				RememberMe = true,
				EventTag = string.Empty
			};
			string url = string.Format ("{0}{1}", Resources.BASE_API_URL, LOGIN_URL);
			return await MakeRequestAsync (url, "POST", string.Empty, model,  new ModernHttpClient.NativeMessageHandler());
		}
	}
}

