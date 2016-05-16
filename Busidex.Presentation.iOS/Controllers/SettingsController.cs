using System;
using Foundation;
using System.Linq;
using Busidex.Mobile;
using UIKit;
using Busidex.Mobile.Models;
using GoogleAnalytics.iOS;
using System.Threading.Tasks;
using System.Net;

namespace Busidex.Presentation.iOS
{
	public partial class SettingsController : BaseController
	{
		public SettingsController (IntPtr handle) : base (handle)
		{
		}

		void SetEmailChangedResult (string email, string result, ref NSUserDefaults user)
		{

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

				var loginContrller = new Busidex.Mobile.LoginController ();
				loginContrller.DoLogin (email, password).ContinueWith (response => {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

					var userId = loginResponse != null ? loginResponse.UserId : 0;

					SetAuthCookie (userId);

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

		static void OpenBrowser ()
		{
			
			try {
				var cardFileId = UISubscriptionService.CurrentUser.CardFileId;

				var url = (string.IsNullOrEmpty (cardFileId) || cardFileId == Guid.Empty.ToString ()) ? Resources.MY_CARD_ADD_URL : Resources.MY_CARD_EDIT_URL;
				url = string.Format (url, WebUtility.UrlEncode (UISubscriptionService.AuthToken));

				UIApplication.SharedApplication.OpenUrl (new NSUrl (url));

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		void HideStatusIndicators ()
		{
			imgEmailSaved.Hidden = lblEmailError.Hidden = true;
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Settings");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			txtEmail.ValueChanged += delegate {
				HideStatusIndicators ();
			};

			btnMyCard.TouchUpInside += delegate {
				//OpenBrowser ();
				GoToCardEdit ();
			};

			btnTerms.TouchUpInside += delegate {
				GoToTerms ();
			};
		}

		async Task<bool> SaveSettings ()
		{
			HideStatusIndicators ();

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			string newEmail = txtEmail.Text;
			string token;
			var user = NSUserDefaults.StandardUserDefaults;

			if (cookie != null) {
				token = cookie.Value;

				string oldEmail = user.StringForKey (Resources.USER_SETTING_EMAIL);

				if (!oldEmail.Equals (newEmail)) {
					await Busidex.Mobile.SettingsController.ChangeEmail (newEmail, token).ContinueWith (emailResponse => {
						InvokeOnMainThread (() => {
							var emailResult = emailResponse.Result;
							SetEmailChangedResult (newEmail, emailResult, ref user);
						});
					});
				}
			} 
			return true;
		}

		static string UpdateUserEmailSetting (NSUserDefaults user)
		{
			string email = user.StringForKey (Resources.USER_SETTING_EMAIL);
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);
			if (cookie != null) {
				var busidexAccountResponse = AccountController.GetAccount (cookie.Value);
				var busidexAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (busidexAccountResponse);
				if (busidexAccount != null) {
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

			HideStatusIndicators ();

			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIBarButtonSystemItem.Save, (sender, args) => SaveSettings ())
					, true);
			}

			string oldEmail;

			if (!string.IsNullOrEmpty (UISubscriptionService.AuthToken)) {
				var user = NSUserDefaults.StandardUserDefaults;

				oldEmail = user.StringForKey (Resources.USER_SETTING_EMAIL);
				if (oldEmail != null && oldEmail.IndexOf ("@", StringComparison.Ordinal) < 0) {
					oldEmail = UpdateUserEmailSetting (user);
				}
				txtEmail.Text = oldEmail;
			}

			txtEmail.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true; 
			};

		}

		// https://developer.xamarin.com/guides/ios/platform_features/introduction_to_ios9/contacts/
		//		void updateContacts(){
		//
		//			if(Application.MyBusidex != null){
		//
		//				var predicate = CNContact.GetPredicateForContacts("");
		//				var fetchKeys = new NSString[] {
		//					CNContactKey.ThumbnailImageData,
		//					CNContactKey.ImageData,
		//					CNContactKey.EmailAddresses,
		//					CNContactKey.PhoneNumbers,
		//					CNContactKey.GivenName
		//				};
		//
		//				var store = new CNContactStore();
		//				NSError error;
		//				var contacts = store.GetUnifiedContacts(predicate, fetchKeys, out error);
		//
		//				foreach(var card in Application.MyBusidex){
		//
		//					var thisContact = contacts.FirstOrDefault (c => c.EmailAddresses.Equals (card.Card.Email) ||
		//						c.PhoneNumbers.Intersect (card.Card.PhoneNumbers.Select (p => p.Number)).Count > 0);
		//
		//					if(thisContact != null){
		//						var mutable = thisContact.MutableCopy() as CNMutableContact;
		//					}
		//				}
		//			}
		//		}
	}
}
