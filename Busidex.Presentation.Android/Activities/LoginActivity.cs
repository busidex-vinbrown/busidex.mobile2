
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
		TextView txtLoginFailed { get; set; }
		TextView txtUserName { get; set; }
		TextView txtPassword { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Login);

			base.OnCreate (savedInstanceState);

			txtLoginFailed = FindViewById<TextView> (Resource.Id.txtLoginFailed);
			txtUserName = FindViewById<TextView> (Resource.Id.txtUserName);
			txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);

			txtLoginFailed.Visibility = global::Android.Views.ViewStates.Gone;



			var button = FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				DoLogin();
			};

			txtUserName.TextChanged += (sender, e) => HideErrorMessage();
			txtPassword.TextChanged += (sender, e) => HideErrorMessage();
		}

		void HideErrorMessage(){
			txtLoginFailed.Visibility = global::Android.Views.ViewStates.Gone;
		}

		void DoLogin(){

			var userName = txtUserName.Text;
			var password = txtPassword.Text;

			var response = Busidex.Mobile.LoginController.DoLogin (userName, password);
			if (!response.Contains ("404")) {
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);
				var userId = loginResponse != null ? loginResponse.UserId : 0;
				applicationResource.SetAuthCookie (userId);

				RedirectToMainIfLoggedIn ();
			}else{
				txtLoginFailed.Visibility = global::Android.Views.ViewStates.Visible;
			}
		}


	}
}

