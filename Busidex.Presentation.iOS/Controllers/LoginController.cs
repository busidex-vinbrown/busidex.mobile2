using System;
using System.Threading.Tasks;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using CoreGraphics;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class LoginController : BaseController
	{
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
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Login");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var cookie = Application.GetAuthCookie ();
			long userId;
			if (cookie != null) {
				userId = Utils.DecodeUserId (cookie.Value);
				if (userId > 0) {
					UserId = userId;
				} 
			} 

			btnForgotPassword.TouchUpInside += delegate {
				UIApplication.SharedApplication.OpenUrl (new NSUrl (Resources.FORGOT_PASSWORD_URL));
			};

			btnForgotUserName.Hidden = true;// not implemented yet
			btnForgotUserName.TouchUpInside += delegate {
				UIApplication.SharedApplication.OpenUrl (new NSUrl (Resources.FORGOT_USERNAME_URL));
			};

			btnLogin.TouchUpInside += async (o, s) => await DoLogin ();
		}

		bool loggingIn;

		void spinImage ()
		{
			UIView.AnimateNotify (.5, 0, UIViewAnimationOptions.Autoreverse | UIViewAnimationOptions.CurveEaseOut, () => {
				imgLogo.Transform = CGAffineTransform.MakeScale (0.01f, 1.1f);
			}, finished => {
				if (loggingIn) {
					spinImage ();
				} else {
					imgLogo.Transform = CGAffineTransform.MakeScale (1.0f, 1.0f);
				}
			});
		}

		async Task<bool> DoLogin ()
		{
			try {
				lblLoginResult.Text = string.Empty;

				string username = txtUserName.Text;
				string password = txtPassword.Text;

				// dismiss the keyboard
				txtPassword.ResignFirstResponder ();
				txtUserName.ResignFirstResponder ();

				loggingIn = true;

				spinImage ();
				var _loginController = new Mobile.LoginController ();
				await _loginController.DoLogin (username, password).ContinueWith (async response => {
					string result = await response;
					if (string.IsNullOrEmpty (result)) {
						InvokeOnMainThread (() => {
							loggingIn = false;
							lblLoginResult.Text = "Login Failed";
							lblLoginResult.TextColor = UIColor.Red;
						});
						return false;
					}

					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (result);
					if (loginResponse == null) {
						InvokeOnMainThread (() => {
							loggingIn = false;
							lblLoginResult.Text = "Login Failed";
							lblLoginResult.TextColor = UIColor.Red;
						});
						return false;
					}

					if (loginResponse.UserId > 0) {

						UserId = loginResponse != null ? loginResponse.UserId : 0;

						if (UserId > 0) {

						    Application.SetAuthCookie (UserId);

							UISubscriptionService.LoadUser ();

							var user = NSUserDefaults.StandardUserDefaults;

							user.SetString (username, Resources.USER_SETTING_USERNAME);
							user.SetString (password, Resources.USER_SETTING_PASSWORD);
							user.SetString (username, Resources.USER_SETTING_EMAIL);
							user.SetBool (true, Resources.USER_SETTING_AUTOSYNC);
							user.Synchronize ();

							// AppDelegate.SetUpPurchases ();
							
							loggingIn = false;

							var quickShareLink = Utils.GetQuickShareLink ();
							if (quickShareLink != null) {
								InvokeOnMainThread (()=> ((BaseNavigationController)NavigationController).GoToQuickShare(quickShareLink));
							} else {
								InvokeOnMainThread (GoToMain);
							}

							return true;
						}
					}
					InvokeOnMainThread (() => {
						loggingIn = false;
						lblLoginResult.Text = "Login Failed";
						lblLoginResult.TextColor = UIColor.Red;
					});
					return true;
				});
					
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
				InvokeOnMainThread (() => {
					Application.ShowAlert ("Login Error", "There was a problem logging in.", new []{ "Ok" });
					loggingIn = false;	
				});
			} 
			return true;
		}
	}
}
