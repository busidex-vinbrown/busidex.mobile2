using System;
using Busidex.Mobile;
using Foundation;
using Busidex.Mobile.Models;
using UIKit;
using System.Threading.Tasks;
using Google.Analytics;
using BaseController = Busidex.Presentation.iOS.Controllers.BaseController;

namespace Busidex.Presentation.iOS
{
	public partial class CreateProfileController : BaseController
	{
		bool termsAccepted;
		const float TERMS_NOT_ACCEPTED_DISPLAY = .0f;
		const float TERMS_ACCEPTED_DISPLAY = 1f;
		LoadingOverlay overlay;

		public CreateProfileController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIBarButtonSystemItem.Save, (sender, args) => SaveSettings ())
					, true);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "CreateProfile");
			HideStatusIndicators ();
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			imgAccept.Alpha = termsAccepted ? TERMS_ACCEPTED_DISPLAY : TERMS_NOT_ACCEPTED_DISPLAY;

			txtPassword.ShouldReturn += textField => { 
				textField.ResignFirstResponder ();
				return true; 
			};

			txtEmail.ValueChanged += delegate {
				HideStatusIndicators ();
			};

			txtPassword.ValueChanged += delegate {
				HideStatusIndicators ();
			};

			btnAcceptTerms.TouchUpInside += delegate {
				handleTermsClick ();	
			}; 

			btnTerms.TouchUpInside += delegate {
				GoToTerms ();
			};

			txtEmail.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true; 
			};
		}

		async Task<bool> SaveSettings ()
		{
			HideStatusIndicators ();

			var user = NSUserDefaults.StandardUserDefaults;

			if (termsAccepted) {
				if (string.IsNullOrEmpty (txtEmail.Text) || string.IsNullOrEmpty (txtPassword.Text)) {
					await Application.ShowAlert ("Email and Password", "Please add your email and password to continue", "Ok");
				} else if (txtEmail.Text.IndexOf ('@') < 0) {
					await Application.ShowAlert ("Email and Password", "Please add a valid email address to continue", "Ok");
				} else {
					var token = Guid.NewGuid ().ToString ();

					var newEmail = txtEmail.Text;
					var newPassword = txtPassword.Text;

					overlay = new LoadingOverlay (View.Bounds);
					overlay.MessageText = "Saving your information";
					View.AddSubview (overlay);

					await AccountController.CheckAccount (token, newEmail, newPassword).ContinueWith (response => {

						if (!response.IsFaulted && !string.IsNullOrEmpty (response.Result)) {
							UISubscriptionService.Init ();
							InvokeOnMainThread (() => {
								overlay.Hide ();
								SetCheckAccountResult (newEmail, newPassword, response.Result, ref user);
							});
						} else {
							InvokeOnMainThread (() => {
								Application.ShowAlert ("Saving your information", "There was a problem saving your information. There may be an issue with your internet connection. Please try again later.", "Ok");
								overlay.Hide ();
							});
						}
					});

				}
			} else {
				await Application.ShowAlert ("Terms and Conditions", "Please accept the terms and conditions to continue", "Ok");
			}

			return true;
		}

		void SetCheckAccountResult (string email, string password, string result, ref NSUserDefaults user)
		{
			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
			const string ERROR_ACCOUNT_EXISTS = "account already exists";
			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, StringComparison.Ordinal) >= 0) {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			} else if (result.ToLowerInvariant ().IndexOf (ERROR_ACCOUNT_EXISTS, StringComparison.Ordinal) >= 0) {
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

				var loginContrller = new Mobile.LoginController ();
				loginContrller.DoLogin (email, password).ContinueWith (response => {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

					var userId = loginResponse != null ? loginResponse.UserId : 0;

					Application.SetAuthCookie (userId);

					InvokeOnMainThread (() => {
						GoToMain ();
						HideStatusIndicators ();
					});
				});

			} else {
				imgEmailSaved.Hidden = true;
				lblEmailError.Hidden = false;
				lblEmailError.Text = "There was a problem updating your account";
			}
		}

		void HideStatusIndicators ()
		{
			imgEmailSaved.Hidden = imgPasswordSaved.Hidden = lblEmailError.Hidden = lblPasswordError.Hidden = true;
		}

		void handleTermsClick ()
		{
			termsAccepted = !termsAccepted;
			imgAccept.Alpha = termsAccepted ? TERMS_ACCEPTED_DISPLAY : TERMS_NOT_ACCEPTED_DISPLAY;
		}


	}
}


