using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class LoginController : BaseController
	{
		LoadingOverlay loadingOverlay;

		public LoginController (IntPtr handle) : base (handle)
		{
		}

		public string DataObject {
			get;
			set;
		}

		static long UserId{ get; set; }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.Title = "Login";
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.SetToolbarHidden (true, false);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Login");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var cookie = GetAuthCookie ();
			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId > 0) {
					UserId = userId;
				} 
			} 

			btnLogin.TouchUpInside += (o, s) => DoLogin ();
		}

		private void DoLogin(){
			try {
				lblLoginResult.Text = string.Empty;

				loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				View.Add (loadingOverlay);

				string username = txtUserName.Text;
				string password = txtPassword.Text;

				var response = Busidex.Mobile.LoginController.DoLogin (username, password);
				if (!string.IsNullOrEmpty (response) && !response.Contains ("404")) {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);

					UserId = loginResponse != null ? loginResponse.UserId : 0;

					if (UserId > 0) {

						SetAuthCookie (UserId);

						var user = NSUserDefaults.StandardUserDefaults;

						user.SetString (username, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
						user.SetString (password, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
						user.SetString (username, Busidex.Mobile.Resources.USER_SETTING_EMAIL);
						user.SetBool (true, Busidex.Mobile.Resources.USER_SETTING_AUTOSYNC);
						user.Synchronize ();

						GoToHome ();
						return;
					}
				}
				lblLoginResult.Text = "Login Failed";
				lblLoginResult.TextColor = UIColor.Red;

			} catch (Exception ex) {
				ShowAlert ("Login Error", "There was a problem logging in.", new string[]{ "Ok" });
			} finally {
				if (loadingOverlay != null) {
					loadingOverlay.Hide ();
				}
			}
		}

		private void GoToHome ()
		{

			var dataViewController = this.Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				this.NavigationController.PushViewController (dataViewController, true);

			}
		}
	}
}
