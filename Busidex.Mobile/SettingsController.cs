using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class SettingsController : BaseController
	{
		private const string CHANGE_USERNAME_URL = "User/ChangeUserName";
		private const string CHANGE_PASSWORD_URL = "Password";
		private const string CHANGE_EMAIL_URL = "User/ChangeEmail";

		public SettingsController ()
		{
		}

		public static Task<string> ChangeUserName(string name, string userToken){

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_USERNAME_URL + "?userId=0&name=" + name;
			string data = "{}";
			return MakeRequest (url, "PUT", userToken, data);
		}

		public static Task<string> ChangePassword(string oldPassword, string newPassword, string userToken){
		
			string data = @"{'UserId':0, 'OldPassword':'" + oldPassword + "','NewPassword':'" + newPassword + "','ConfirmPassword':'" + newPassword + "'}";

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_PASSWORD_URL;

			return MakeRequest (url, "PUT", userToken, data);
		}

		public static Task<string> ChangeEmail(string email, string userToken){

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_EMAIL_URL + "?email=" + email;
			string data = "{}";
			return MakeRequest (url, "PUT", userToken, data);
		}
	}
}