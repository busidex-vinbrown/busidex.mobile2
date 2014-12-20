using System;
using System.Net;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class LoginController: BaseController
	{
		const string TEST_ACCOUNT_ID = "45ef6a4e-3c9d-452c-9409-9e4d7b6e532f";
		const string LOGIN_URL = "Account/Login";
		const string CHECK_ACCOUNT_URL = "Registration/CheckAccount";
		const bool DEVELOPMENT_MODE = true;


		public static long DoLogin(string username, string password){
		
			string data = "{'UserName':'" + username + "','Password':'" + password + "','Token':'','RememberMe':'true'}";

			return login (string.Format ("{0}{1}", Resources.BASE_API_URL, LOGIN_URL), data, "application/json");
		}

		static long login(string url, string data, string contentType){

			long userId = 0;

			var request = WebRequest.Create (url) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = contentType;
			request.ContentLength = data.Length;
			var requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
			requestWriter.Write(data);
			requestWriter.Close();

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				var responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);

				userId = loginResponse != null ? loginResponse.UserId : 0;

				responseReader.Close();

			} catch (Exception e) {
				LoggingController.LogError (e, string.Empty);
			}

			return userId;

		}
	}
}

