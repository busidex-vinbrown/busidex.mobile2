

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Views;
using Android.Net;
using Android.Content;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Profile")]			
	public class ProfileActivity : BaseActivity
	{
		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		ImageView imgAcceptTerms;

		TextView lblEmailError;
		TextView lblPasswordError;

		bool showPassword = true;
		bool termsAccepted;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			
			base.OnCreate (savedInstanceState);

			//Remove title bar
			RequestWindowFeature(WindowFeatures.NoTitle);

			SetContentView (Resource.Layout.Profile);

			var txtProfileEmail = FindViewById<TextView> (Resource.Id.txtProfileEmail);
			var txtProfilePassword = FindViewById<TextView> (Resource.Id.txtProfilePassword);
			var txtProfileDescription = FindViewById<TextView> (Resource.Id.txtProfileDescription);
			var lblProfilePassword = FindViewById<TextView> (Resource.Id.lblProfilePassword);
			var txtAcceptTerms = FindViewById<TextView> (Resource.Id.txtAcceptTerms);
			var txtViewTerms = FindViewById<TextView> (Resource.Id.txtViewTerms);
			imgAcceptTerms = FindViewById<ImageView> (Resource.Id.imgAcceptTerms);
			imgProfileEmailSaved = FindViewById<ImageView> (Resource.Id.imgProfileEmailSaved);
			imgProfilePasswordSaved = FindViewById<ImageView> (Resource.Id.imgProfilePasswordSaved);
			lblEmailError = FindViewById<TextView> (Resource.Id.lblEmailError);
			lblPasswordError = FindViewById<TextView> (Resource.Id.lblPasswordError);

			lblPasswordError.Visibility = ViewStates.Invisible;
			lblEmailError.Visibility = ViewStates.Invisible;

			var btnSaveProfile = FindViewById<Button> (Resource.Id.btnSaveProfile);

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

				txtAcceptTerms.Click += delegate {
					toggleTerms();
				};

				imgAcceptTerms.Click += delegate {
					toggleTerms();
				};


				txtViewTerms.Click += delegate {

					var uri = Uri.Parse (Busidex.Mobile.Resources.TERMS_AND_CONDITIONS_URL);
					var OpenBrowserIntent = new Intent (Intent.ActionView);
					OpenBrowserIntent.SetData (uri);

					var browserIntent = Intent.CreateChooser(OpenBrowserIntent, "Open with");
					StartActivity (browserIntent);

				};

				btnSaveProfile.Click += async delegate {

					var email = txtProfileEmail.Text;
					if(!email.Contains("@")){
						//ShowAlert("User Information", "Please use a valid email address.");
						return;
					}

					if(string.IsNullOrEmpty(txtProfilePassword.Text)){
						//ShowAlert("User Information", "Please enter a password.");
						return;
					}
					if(termsAccepted){
						await CheckAccount(token, txtProfileEmail.Text, txtProfilePassword.Text);
					}else{
						//ShowAlert("Terms and Conditions", "Please accept the terms and conditions to continue.");
					}
				};
			}
		}

		void toggleTerms(){
			termsAccepted = !termsAccepted;
			imgAcceptTerms.Alpha = termsAccepted ? 1 : .4f;
		}

		void SetEmailChangedResult(string result){

			if (result.IndexOf ("400", System.StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
			} else if (result.ToLowerInvariant ().IndexOf ("email updated", System.StringComparison.Ordinal) >= 0) {
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

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, System.StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			}else if (result.ToLowerInvariant ().IndexOf (GetString(Resource.String.Profile_ErrorAccountExists), System.StringComparison.Ordinal) >= 0) {
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

				//RunOnUiThread (() => ShowLoadingSpinner (Resources.GetString (Resource.String.Global_LoggingYouIn)));

				LoginController.DoLogin(email, password).ContinueWith(response => {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

					var userId = loginResponse != null ? loginResponse.UserId : 0;

					applicationResource.SetAuthCookie (userId);

					RunOnUiThread ( ()=> {
						//HideLoadingSpinner();
						RedirectToMainIfLoggedIn();
					});	
				});
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

		async Task<bool> CheckAccount(string token, string email, string password){
			token = System.Guid.NewGuid ().ToString ();

			//RunOnUiThread (() => ShowLoadingSpinner (Resources.GetString (Resource.String.Global_OneMoment)));

			await AccountController.CheckAccount (token, email, password).ContinueWith (response => {

				RunOnUiThread( ()=> {
					//HideLoadingSpinner();
					SetCheckAccountResult (email, password, response.Result);
				});

			});


			return true;
		}
	}
}

