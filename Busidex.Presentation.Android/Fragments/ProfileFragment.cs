
using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Android
{
	public class ProfileFragment : BaseFragment
	{
		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		TextView lblEmailError;
		TextView lblPasswordError;
		bool showPassword = true;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
		
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			View profileView = inflater.Inflate(Resource.Layout.Profile, container, false);

			var txtProfileEmail = profileView.FindViewById<TextView> (Resource.Id.txtProfileEmail);
			var txtProfilePassword = profileView.FindViewById<TextView> (Resource.Id.txtProfilePassword);
			var txtProfileDescription = profileView.FindViewById<TextView> (Resource.Id.txtProfileDescription);
			var lblProfilePassword = profileView.FindViewById<TextView> (Resource.Id.lblProfilePassword);
			imgProfileEmailSaved = profileView.FindViewById<ImageView> (Resource.Id.imgProfileEmailSaved);
			imgProfilePasswordSaved = profileView.FindViewById<ImageView> (Resource.Id.imgProfilePasswordSaved);
			lblEmailError = profileView.FindViewById<TextView> (Resource.Id.lblEmailError);
			lblPasswordError = profileView.FindViewById<TextView> (Resource.Id.lblPasswordError);

			var btnSaveProfile = profileView.FindViewById<Button> (Resource.Id.btnSaveProfile);

			applicationResource = new BaseApplicationResource (Activity);

			var token = applicationResource.GetAuthCookie ();
			var accountJSON = AccountController.GetAccount (token);
			var account = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

			imgProfileEmailSaved.Visibility = imgProfilePasswordSaved.Visibility = ViewStates.Invisible;

			if(account != null){
				txtProfileEmail.Text = account.Email;
				txtProfilePassword.Visibility = imgProfilePasswordSaved.Visibility = lblPasswordError.Visibility = ViewStates.Gone;
				lblEmailError.Visibility = lblProfilePassword.Visibility = ViewStates.Gone;

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

			return profileView;
		}

		void SetEmailChangedResult(string result){

			if (result.IndexOf ("400", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
			} else if (result.ToLowerInvariant ().IndexOf ("email updated", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Visible;
				lblEmailError.Visibility = ViewStates.Invisible;
			} else {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText(Resource.String.Profile_ErrorEmailGeneral);
			}
		}

		void SetCheckAccountResult(string email, string password, string result){
			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			}else if (result.ToLowerInvariant ().IndexOf (GetString(Resource.String.Profile_ErrorAccountExists), StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.Text = "This email is already in use";
			} else if (oResult != null && oResult.Success) {
				imgProfileEmailSaved.Visibility = ViewStates.Visible;
				lblEmailError.Visibility = ViewStates.Invisible;
				if (showPassword) {
					imgProfilePasswordSaved.Visibility = ViewStates.Visible;
					lblPasswordError.Visibility = ViewStates.Invisible;
				}
				var response = LoginController.DoLogin(email, password);
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

				var userId = loginResponse != null ? loginResponse.UserId : 0;

				applicationResource.SetAuthCookie (userId);

				RedirectToMainIfLoggedIn ();

			} else {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
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

