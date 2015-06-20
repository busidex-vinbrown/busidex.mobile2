using System;
using Foundation;
using System.Linq;
using Busidex.Mobile;
using UIKit;
using Busidex.Mobile.Models;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class SettingsController : BaseController
	{
		public SettingsController (IntPtr handle) : base (handle)
		{
		}

		bool termsAccepted;
		const float TERMS_NOT_ACCEPTED_DISPLAY = .2f;
		const float TERMS_ACCEPTED_DISPLAY = 1f;
		bool loggedIn;

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

		void SetPasswordChangedResult(string password, string result, ref NSUserDefaults user){

			if (result.ToLowerInvariant ().IndexOf ("password changed", StringComparison.Ordinal) >= 0) {
				user.SetString (password, Resources.USER_SETTING_PASSWORD);
				user.Synchronize ();
				imgPasswordSaved.Hidden = false;
				lblPasswordError.Hidden = true;
			} else {
				imgPasswordSaved.Hidden = true;
				lblPasswordError.Hidden = false;
				lblPasswordError.Text = "There was a problem saving your password";
			}
		}

		void SetEmailChangedResult(string email, string result, ref NSUserDefaults user){

			if (result.IndexOf ("400", StringComparison.Ordinal) >= 0) {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "Email is already in use";
			} else if (result.ToLowerInvariant ().IndexOf ("email updated", StringComparison.Ordinal) >= 0) {
				user.SetString (email, Resources.USER_SETTING_EMAIL);
				user.Synchronize ();
				imgEmailSaved.Hidden = false;
				lblEmailError.Hidden = true;
			} else {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "There was a problem saving your email";
			}
		}

		void SetCheckAccountResult(string email, string password, string result, ref NSUserDefaults user){
			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
			const string ERROR_ACCOUNT_EXISTS = "account already exists";
			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, StringComparison.Ordinal) >= 0) {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			}else if (result.ToLowerInvariant ().IndexOf (ERROR_ACCOUNT_EXISTS, StringComparison.Ordinal) >= 0) {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "This email is already in use";
			} else if (oResult != null && oResult.Success) {
				user.SetString (email, Resources.USER_SETTING_EMAIL);
				user.SetString (password, Resources.USER_SETTING_PASSWORD);
				user.Synchronize ();
				imgEmailSaved.Hidden = false;
				lblEmailError.Hidden = true;
				imgPasswordSaved.Hidden = false;
				lblPasswordError.Hidden = true;

				Busidex.Mobile.LoginController.DoLogin(email, password).ContinueWith(response => {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

					var userId = loginResponse != null ? loginResponse.UserId : 0;

					SetAuthCookie (userId);

					InvokeOnMainThread (GoToMain);
				});


			} else {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "There was a problem updating your account";
			}
		}

		void HideStatusIndicators(){
			imgEmailSaved.Hidden = imgPasswordSaved.Hidden = lblEmailError.Hidden = lblPasswordError.Hidden = true;
		}
			
		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Settings");

			base.ViewDidAppear (animated);
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

			btnAcceptTerms.TouchUpInside += delegate {
				handleTermsClick();	
			}; 

			btnTerms.TouchUpInside += delegate {
				showTerms();
			};
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			UITextView lblInstructions = new UITextView ();
			lblInstructions.BackgroundColor = UIColor.Clear;
			lblInstructions.UserInteractionEnabled = false;
			lblInstructions.TextColor = UIColor.DarkGray;
			lblInstructions.Text = loggedIn 
				? "Update your email address so you can access your cards on the web and all your devices."
				: "Choose an email address and password so you can access your cards on the web and all your devices.";
			
			lblInstructions.TextAlignment = UITextAlignment.Justified;

			float height = loggedIn ? 65f : 95f;
			var frame = new CoreGraphics.CGRect (
				padding.Frame.X, 
				padding.Frame.Y, 
				padding.Frame.Width, 
				height);

			padding.Frame = frame;

			lblInstructions.Frame = new CoreGraphics.CGRect (
				padding.Bounds.X + 10, 
				padding.Bounds.Y + 10, 
				padding.Bounds.Width - 20, 
				padding.Bounds.Height - 10);
			
			padding.AddSubview (lblInstructions);

		}

		void SaveSettings(){
			HideStatusIndicators ();

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			string newPassword = txtPassword.Text;
			string newEmail = txtEmail.Text;
			string token;
			var user = NSUserDefaults.StandardUserDefaults;

			if(cookie != null){
				token = cookie.Value;

				user.StringForKey (Resources.USER_SETTING_USERNAME);
				string oldPassword = user.StringForKey(Resources.USER_SETTING_PASSWORD);
				string oldEmail = user.StringForKey(Resources.USER_SETTING_EMAIL);

				if(!oldPassword.Equals(newPassword)){
					Busidex.Mobile.SettingsController.ChangePassword (oldPassword, newPassword, token).ContinueWith (passwordResponse => {

						InvokeOnMainThread( ()=> {
							var passwordResult = passwordResponse.Result;
							SetPasswordChangedResult(newPassword, passwordResult, ref user);
						});
					});
				}
				if(!oldEmail.Equals(newEmail)){
					Busidex.Mobile.SettingsController.ChangeEmail (newEmail, token).ContinueWith (emailResponse => {
						InvokeOnMainThread( ()=> {
							var emailResult = emailResponse.Result;
							SetEmailChangedResult(newEmail, emailResult, ref user);
						});
					});
				}
			}else{
				if (termsAccepted) {
					if (string.IsNullOrEmpty (txtEmail.Text) || string.IsNullOrEmpty (txtPassword.Text)) {
						ShowAlert ("Email and Password", "Please add your email and password to continue", "Ok");
					}else if(txtEmail.Text.IndexOf('@') < 0){
						ShowAlert ("Email and Password", "Please add a valid email address to continue", "Ok");
					} else {
						token = Guid.NewGuid ().ToString ();
						var response = AccountController.CheckAccount (token, newEmail, newPassword);
						SetCheckAccountResult (newEmail, newPassword, response, ref user);
					}
				}else {
					ShowAlert ("Terms and Conditions", "Please accept the terms and conditions to continue", "Ok");
				}
			}
		}

		static string UpdateUserEmailSetting(NSUserDefaults user){
			string email = user.StringForKey(Resources.USER_SETTING_EMAIL);
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);
			if(cookie != null){
				var busidexAccountResponse = AccountController.GetAccount (cookie.Value);
				var busidexAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (busidexAccountResponse);
				if(busidexAccount != null){
					user.SetString (busidexAccount.Email, Resources.USER_SETTING_EMAIL);
					user.Synchronize ();
					email = busidexAccount.Email;
				}
			}
			return email;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				NavigationItem.SetRightBarButtonItem(
					new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, args) => SaveSettings ())
					, true);
			}


			var user = NSUserDefaults.StandardUserDefaults;

			string oldPassword = user.StringForKey(Resources.USER_SETTING_PASSWORD);
			string oldEmail = user.StringForKey(Resources.USER_SETTING_EMAIL);
			if(oldEmail != null && oldEmail.IndexOf ("@", StringComparison.Ordinal) < 0){
				oldEmail = UpdateUserEmailSetting (user);
			}
			txtPassword.Text = oldPassword;
			txtEmail.Text = oldEmail;

			txtPassword.ShouldReturn += textField => { 
				textField.ResignFirstResponder ();
				return true; 
			};

			txtEmail.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true; 
			};

			if (!string.IsNullOrEmpty (oldEmail)) {
				imgAccept.Hidden = btnTerms.Hidden = btnAcceptTerms.Hidden = true;
				txtPassword.Hidden = lblPassword.Hidden = true;
				loggedIn = true;
			} else {
				loggedIn = false;
				imgAccept.Hidden = btnTerms.Hidden = btnAcceptTerms.Hidden = false;
				txtPassword.Hidden = lblPassword.Hidden = false;

				imgAccept.Alpha = termsAccepted ? TERMS_ACCEPTED_DISPLAY : TERMS_NOT_ACCEPTED_DISPLAY;
			}

		}

		void handleTermsClick(){
			termsAccepted = !termsAccepted;
			imgAccept.Alpha = termsAccepted ? TERMS_ACCEPTED_DISPLAY : TERMS_NOT_ACCEPTED_DISPLAY;
		}

		void showTerms()
		{
			var termsController = Storyboard.InstantiateViewController ("TermsController") as TermsController;

			if (termsController != null && NavigationController.ChildViewControllers.Count (c => c is TermsController) == 0){
				NavigationController.PushViewController (termsController, true);
			}
		}
	}
}
