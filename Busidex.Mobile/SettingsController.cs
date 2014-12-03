using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class SettingsController : BaseController
	{
		const string CHANGE_USERNAME_URL = "User/ChangeUserName";
		const string CHANGE_PASSWORD_URL = "Password";
		const string CHANGE_EMAIL_URL = "User/ChangeEmail";


		public static Task<string> ChangeUserName(string name, string userToken){

			string url = Resources.BASE_API_URL + CHANGE_USERNAME_URL + "?userId=0&name=" + name;
			string data = "{}";
			return MakeRequestAsync (url, "PUT", userToken, data);
		}

		public static Task<string> ChangePassword(string oldPassword, string newPassword, string userToken){
		
			string data = @"{'UserId':0, 'OldPassword':'" + oldPassword + "','NewPassword':'" + newPassword + "','ConfirmPassword':'" + newPassword + "'}";

			string url = Resources.BASE_API_URL + CHANGE_PASSWORD_URL;

			return MakeRequestAsync (url, "PUT", userToken, data);
		}

		public static Task<string> ChangeEmail(string email, string userToken){

			string url = Resources.BASE_API_URL + CHANGE_EMAIL_URL + "?email=" + email;
			string data = "{}";
			return MakeRequestAsync (url, "PUT", userToken, data);
		}
	}
}