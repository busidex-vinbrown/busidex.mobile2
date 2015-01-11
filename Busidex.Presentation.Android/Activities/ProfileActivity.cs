
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Profile")]			
	public class ProfileActivity : BaseActivity
	{
		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		TextView lblEmailError;
		TextView lblPasswordError;
		bool showPassword = true;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Profile);

			base.OnCreate (savedInstanceState);

			var txtProfileEmail = FindViewById<TextView> (Resource.Id.txtProfileEmail);
			var txtProfilePassword = FindViewById<TextView> (Resource.Id.txtProfilePassword);
			var txtProfileDescription = FindViewById<TextView> (Resource.Id.txtProfileDescription);
			var lblProfilePassword = FindViewById<TextView> (Resource.Id.lblProfilePassword);
			imgProfileEmailSaved = FindViewById<ImageView> (Resource.Id.imgProfileEmailSaved);
			imgProfilePasswordSaved = FindViewById<ImageView> (Resource.Id.imgProfilePasswordSaved);
			lblEmailError = FindViewById<TextView> (Resource.Id.lblEmailError);
			lblPasswordError = FindViewById<TextView> (Resource.Id.lblPasswordError);

			var btnSaveProfile = FindViewById<Button> (Resource.Id.btnSaveProfile);

			var token = GetAuthCookie ();
			var accountJSON = AccountController.GetAccount (token);
			var account = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

			imgProfileEmailSaved.Visibility = imgProfilePasswordSaved.Visibility = global::Android.Views.ViewStates.Invisible;

			if(account != null){
				txtProfileEmail.Text = account.Email;
				txtProfilePassword.Visibility = imgProfilePasswordSaved.Visibility = lblPasswordError.Visibility = global::Android.Views.ViewStates.Gone;
				lblEmailError.Visibility = lblProfilePassword.Visibility = global::Android.Views.ViewStates.Gone;

				showPassword = false;
				txtProfileDescription.SetText (Resource.String.Profile_DescriptionUpdateAccount);

				btnSaveProfile.Click += delegate {
					UpdateEmail(token, txtProfileEmail.Text);
				};
			}else{
				txtProfileDescription.SetText (Resource.String.Profile_DescriptionNewAccount);

				btnSaveProfile.Click += delegate {
					CheckAccount(token, txtProfileEmail.Text, txtProfilePassword.Text);
				};
			}
		}

		void SetEmailChangedResult(string result){

			if (result.IndexOf ("400", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Invisible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
			} else if (result.ToLowerInvariant ().IndexOf ("email updated", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Invisible;
			} else {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Invisible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.SetText(Resource.String.Profile_ErrorEmailGeneral);
			}
		}

		void SetCheckAccountResult(string email, string password, string result){
			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Invisible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			}else if (result.ToLowerInvariant ().IndexOf (GetString(Resource.String.Profile_ErrorAccountExists), StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Invisible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.Text = "This email is already in use";
			} else if (oResult != null && oResult.Success) {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Invisible;
				if (showPassword) {
					imgProfilePasswordSaved.Visibility = global::Android.Views.ViewStates.Visible;
					lblPasswordError.Visibility = global::Android.Views.ViewStates.Invisible;
				}
				var response = LoginController.DoLogin(email, password);
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);

				var userId = loginResponse != null ? loginResponse.UserId : 0;

				SetAuthCookie (userId);

				RedirectToMainIfLoggedIn ();

			} else {
				imgProfileEmailSaved.Visibility = global::Android.Views.ViewStates.Invisible;
				lblEmailError.Visibility = global::Android.Views.ViewStates.Visible;
				lblEmailError.Text = GetString (Resource.String.Profile_ErrorAccountGeneral);
			}
		}

		void UpdateEmail(string token, string email){
			var response = SettingsController.ChangeEmail (email, token);
			SetEmailChangedResult (response.Result);
		}

		void CheckAccount(string token, string email, string password){
			token = Guid.NewGuid ().ToString ();
			var response = AccountController.CheckAccount (token, email, password);
			SetCheckAccountResult (email, password, response);
		}
	}
}

