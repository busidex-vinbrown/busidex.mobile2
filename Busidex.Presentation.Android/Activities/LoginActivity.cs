
using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Sign In")]			
	public class LoginActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Login);

			var button = FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				DoLogin();
			};
		}

		void DoLogin(){

			var txtUserName = FindViewById<TextView> (Resource.Id.txtUserName);
			var txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);

			var userName = txtUserName.Text;
			var password = txtPassword.Text;

			var response = Busidex.Mobile.LoginController.DoLogin (userName, password);
			var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);
			var userId = loginResponse != null ? loginResponse.UserId : 0;
			SetAuthCookie (userId);

			RedirectToMainIfLoggedIn ();
		}


	}
}

