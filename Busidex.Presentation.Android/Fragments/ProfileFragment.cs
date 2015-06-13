
using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Views.Animations;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android.Fragments
{
	public class ProfileFragment : BaseFragment
	{

		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		TextView lblEmailError;
		TextView lblPasswordError;
		bool showPassword = true;

		GestureDetector.IOnGestureListener _detector;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
		
			_detector = (MainActivity)this.Activity;

			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			View profileView = inflater.Inflate(Resource.Layout.Profile, container, false);

			var txtAcceptTerms = profileView.FindViewById<TextView> (Resource.Id.txtAcceptTerms);
			var txtViewTerms = profileView.FindViewById<TextView> (Resource.Id.txtViewTerms);
			var imgAcceptTerms = profileView.FindViewById<ImageView> (Resource.Id.imgAcceptTerms);

			txtAcceptTerms.Visibility = txtViewTerms.Visibility = imgAcceptTerms.Visibility = ViewStates.Gone;

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

				btnSaveProfile.Click += async delegate {
					await UpdateEmail (token, txtProfileEmail.Text);
				};
			}/*else{
				txtProfileDescription.SetText (Resource.String.Profile_DescriptionNewAccount);

				btnSaveProfile.Click += delegate {
					CheckAccount(token, txtProfileEmail.Text, txtProfilePassword.Text);
				};
			}*/

			return profileView;
		}

		bool SetEmailChangedResult(string result){

			if (result.IndexOf ("400", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
				return false;
			} else if (result.ToLowerInvariant ().IndexOf ("email updated", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Visible;
				lblEmailError.Visibility = ViewStates.Invisible;
				return true;
			}else if(string.IsNullOrEmpty(result)){
				return true;
			} else {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText(Resource.String.Profile_ErrorEmailGeneral);
				return false;
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

		async Task<bool> UpdateEmail(string token, string email){
			 await SettingsController.ChangeEmail (email, token).ContinueWith(response => {

				this.Activity.RunOnUiThread (() => {
					if(SetEmailChangedResult (response.Result)){
						var slideOut = AnimationUtils.LoadAnimation (this.Activity, Resource.Animation.SlideOutAnimationFast);
						this.View.Visibility = ViewStates.Gone;
						this.View.StartAnimation(slideOut);
						((MainActivity)this.Activity).profileIsOpen = false;
					}
				});
			});
			return true;
		}

		void CheckAccount(string token, string email, string password){
			token = Guid.NewGuid ().ToString ();
			var response = AccountController.CheckAccount (token, email, password);
			SetCheckAccountResult (email, password, response);
		}


	}
}

