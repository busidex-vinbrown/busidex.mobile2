using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Linq;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	partial class SettingsController : BaseController
	{
		public SettingsController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}

//		private void SetUserNameChangedResult(string username, string result, ref NSUserDefaults user){
//			if (result.IndexOf ("400") >= 0) {
//				//imgUserNameSaved.Hidden = true;
//				//lblUserNameError.Hidden = false;
//				//lblUserNameError.Text = "UserName is already in use";
//			} else if (result.ToLowerInvariant().IndexOf ("username updated") >= 0) {
//				user.SetString (username, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
//				user.Synchronize ();
//				imgUserNameSaved.Hidden = false;
//				lblUserNameError.Hidden = true;
//			} else {
//				imgUserNameSaved.Hidden = true;
//				lblUserNameError.Hidden = false;
//				lblUserNameError.Text = "There was a problem saving your UserName";
//			}
//		}

		private void SetPasswordChangedResult(string password, string result, ref NSUserDefaults user){

			if (result.ToLowerInvariant().IndexOf ("password changed") >= 0) {
				user.SetString (password, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
				user.Synchronize ();
				imgPasswordSaved.Hidden = false;
				lblPasswordError.Hidden = true;
			} else {
				imgPasswordSaved.Hidden = true;
				lblPasswordError.Hidden = false;
				lblPasswordError.Text = "There was a problem saving your password";
			}
		}

		private void SetEmailChangedResult(string email, string result, ref NSUserDefaults user){

			if (result.IndexOf ("400") >= 0) {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "Email is already in use";
			} else if (result.ToLowerInvariant().IndexOf ("email updated") >= 0) {
				user.SetString (email, Busidex.Mobile.Resources.USER_SETTING_EMAIL);
				user.Synchronize ();
				imgEmailSaved.Hidden = false;
				lblEmailError.Hidden = true;
			} else {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "There was a problem saving your email";
			}
		}

		private void HideStatusIndicators(){
			imgEmailSaved.Hidden = imgPasswordSaved.Hidden = lblEmailError.Hidden = lblPasswordError.Hidden = true;
		}

		public override void ViewDidLoad ()
		{
			HideStatusIndicators ();

			txtEmail.ValueChanged += delegate {
				HideStatusIndicators ();
			};

			txtPassword.ValueChanged += delegate {
				HideStatusIndicators ();
			};

//			txtUserName.ValueChanged += delegate {
//				HideStatusIndicators ();
//			};

			btnSave.TouchUpInside += delegate {

				HideStatusIndicators ();

				NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME).SingleOrDefault();
				if(cookie != null){
					string token = cookie.Value;

					var user = NSUserDefaults.StandardUserDefaults;
					string oldUserName = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_USERNAME);
					string oldPassword = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
					string oldEmail = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_EMAIL);

					//string newUserName = txtUserName.Text;
					string newPassword = txtPassword.Text;
					string newEmail = txtEmail.Text;

//					if(!oldUserName.Equals(newUserName)){
//						var userNameResponse = Busidex.Mobile.SettingsController.ChangeUserName(newUserName, token);
//						var userNameResult = userNameResponse.Result;
//						SetUserNameChangedResult(newUserName, userNameResult, ref user);
//						//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.USER_NAME_CHANGED, UIMetrics.INTERACTIONS_CATEGORY, new NSNumber (1));
//					}
					if(!oldPassword.Equals(newPassword)){
						var passwordResponse = Busidex.Mobile.SettingsController.ChangePassword(oldPassword, newPassword, token);
						var passwordResult = passwordResponse.Result;
						SetPasswordChangedResult(newPassword, passwordResult, ref user);
						//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.PASSWORD_CHANGED, UIMetrics.INTERACTIONS_CATEGORY, new NSNumber (1));
					}
					if(!oldEmail.Equals(newEmail)){
						var emailResponse = Busidex.Mobile.SettingsController.ChangeEmail(newEmail, token);
						var emailResult = emailResponse.Result;
						SetEmailChangedResult(newEmail, emailResult, ref user);
						//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.EMAIL_CHANGED, UIMetrics.INTERACTIONS_CATEGORY, new NSNumber (1));
					}
				}
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}

			var user = NSUserDefaults.StandardUserDefaults;
			string oldUserName = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_USERNAME);
			string oldPassword = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
			string oldEmail = user.StringForKey(Busidex.Mobile.Resources.USER_SETTING_EMAIL);

			//txtUserName.Text = oldUserName;
			txtPassword.Text = oldPassword;
			txtEmail.Text = oldEmail;
		}
	}
}
