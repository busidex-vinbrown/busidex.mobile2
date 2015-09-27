
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;

namespace Busidex.Presentation.Droid.v2
{
	public class ProfileFragment : GenericViewPagerFragment
	{

		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		ImageView imgAcceptTerms;
		TextView lblEmailError;
		TextView lblPasswordError;
		//TextView txtProfileEmail;
		ImageButton btnLogout;
		RelativeLayout profileCover;
		ProgressBar progress1;
		View profileView;

		bool showPassword = true;
		bool termsAccepted;
		public BusidexUser CurrentUser;

		public ProfileFragment(){
			
		}

		public ProfileFragment(BusidexUser user){
			CurrentUser = user;
		}

		public void UpdateUser(BusidexUser bu){
			CurrentUser = bu;
			updateUI ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			updateUI ();
		}

		void updateUI(){
			if(profileView == null){
				return;
			}

			profileView.RequestLayout ();
			profileCover = profileView.FindViewById<RelativeLayout> (Resource.Id.profileCover);
			progress1 = profileView.FindViewById<ProgressBar> (Resource.Id.progressBar1);

			profileCover.Visibility = progress1.Visibility = ViewStates.Gone;

			var txtAcceptTerms = profileView.FindViewById<TextView> (Resource.Id.txtAcceptTerms);
			var txtViewTerms = profileView.FindViewById<TextView> (Resource.Id.txtViewTerms);
			imgAcceptTerms = profileView.FindViewById<ImageView> (Resource.Id.imgAcceptTerms);

			txtAcceptTerms.Visibility = txtViewTerms.Visibility = imgAcceptTerms.Visibility = ViewStates.Gone;

			var txtProfileEmail = profileView.FindViewById<TextView> (Resource.Id.txtProfileEmail);
			var txtProfilePassword = profileView.FindViewById<TextView> (Resource.Id.txtProfilePassword);
			var txtProfileDescription = profileView.FindViewById<TextView> (Resource.Id.txtProfileDescription);
			var lblProfilePassword = profileView.FindViewById<TextView> (Resource.Id.lblProfilePassword);
			imgProfileEmailSaved = profileView.FindViewById<ImageView> (Resource.Id.imgProfileEmailSaved);
			imgProfilePasswordSaved = profileView.FindViewById<ImageView> (Resource.Id.imgProfilePasswordSaved);
			lblEmailError = profileView.FindViewById<TextView> (Resource.Id.lblEmailError);
			lblPasswordError = profileView.FindViewById<TextView> (Resource.Id.lblPasswordError);
			btnLogout = profileView.FindViewById<ImageButton> (Resource.Id.btnLogout);

			lblPasswordError.Visibility = ViewStates.Invisible;
			lblEmailError.Visibility = ViewStates.Invisible;

			var btnSaveProfile = profileView.FindViewById<Button> (Resource.Id.btnSaveProfile);

			btnLogout.Click += delegate {
				Logout();
			};

			var token = BaseApplicationResource.GetAuthCookie ();

			imgProfileEmailSaved.Visibility = imgProfilePasswordSaved.Visibility = ViewStates.Invisible;

			if(CurrentUser != null){
				txtProfileEmail.Text = CurrentUser.Email;
				txtProfileEmail.RequestLayout ();
				txtProfileEmail.Click += (object sender, System.EventArgs e) => {
					ShowAlert("WTF?", "The current user's email is " + CurrentUser.Email, "Ok", null);
					txtProfileEmail.Text = CurrentUser.Email;
				};
				txtProfilePassword.Visibility = imgProfilePasswordSaved.Visibility = lblPasswordError.Visibility = ViewStates.Gone;
				lblEmailError.Visibility = lblProfilePassword.Visibility = ViewStates.Gone;

				showPassword = false;
				txtProfileDescription.SetText (Resource.String.Profile_DescriptionUpdateAccount);

				btnSaveProfile.Click += async delegate {
					await UpdateEmail (token, txtProfileEmail.Text);
				};
				profileView.RequestLayout ();
			}else{
				txtProfileEmail.Text = string.Empty;

				btnLogout.Visibility = ViewStates.Gone;

				txtAcceptTerms.Visibility = txtViewTerms.Visibility = imgAcceptTerms.Visibility = ViewStates.Visible;

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
						ShowAlert("User Information", "Please use a valid email address.", GetString(Resource.String.Global_ButtonText_Ok), null);
						return;
					}

					if(string.IsNullOrEmpty(txtProfilePassword.Text)){
						ShowAlert("User Information", "Please enter a password.", GetString(Resource.String.Global_ButtonText_Ok), null);
						return;
					}
					if(termsAccepted){
						await CheckAccount(token, txtProfileEmail.Text, txtProfilePassword.Text);
					}else{
						ShowAlert("Terms and Conditions", "Please accept the terms and conditions to continue.", GetString(Resource.String.Global_ButtonText_Ok), null);
					}
				};
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
		
			profileView = inflater.Inflate(Resource.Layout.Profile, container, false);

			//updateUI ();

			return profileView;
		}

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();
			//txtProfileEmail = null;
			profileView = null;
		}

		void toggleTerms(){
			termsAccepted = !termsAccepted;
			imgAcceptTerms.Alpha = termsAccepted ? 1 : .4f;
		}

		bool SetEmailChangedResult(string result)
		{

			if (result.IndexOf ("400", System.StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
				return false;
			}

			if (result.ToLowerInvariant ().IndexOf ("email updated", System.StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Visible;
				lblEmailError.Visibility = ViewStates.Invisible;
				return true;
			}

			if (string.IsNullOrEmpty (result)) {
				return true;
			}

			imgProfileEmailSaved.Visibility = ViewStates.Invisible;
			lblEmailError.Visibility = ViewStates.Visible;
			lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
			return false;
		}

		void SetCheckAccountResult(string email, string password, string result){
			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);

			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, System.StringComparison.Ordinal) >= 0) {
				displayCover (false);
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
			}else if (result.ToLowerInvariant ().IndexOf (GetString(Resource.String.Profile_ErrorAccountExists), System.StringComparison.Ordinal) >= 0) {
				displayCover (false);
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
				LoginController.DoLogin(email, password).ContinueWith(response => {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);

					var userId = loginResponse != null ? loginResponse.UserId : 0;

					BaseApplicationResource.SetAuthCookie (userId);

					Activity.RunOnUiThread(() => ((MainActivity)Activity).LoginComplete ());
				});
			} else {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.Text = GetString (Resource.String.Profile_ErrorAccountGeneral);
			}
		}

		async Task<bool> UpdateEmail(string token, string email){
			 await SettingsController.ChangeEmail (email, token).ContinueWith(response => {

				Activity.RunOnUiThread (() => {
					if(SetEmailChangedResult (response.Result)){
						((MainActivity)Activity).UnloadFragment();
					}
				});
			});
			return true;
		}

		void displayCover(bool visible){

			profileCover.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
			progress1.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
		}

		async Task<bool> CheckAccount(string token, string email, string password){
			token = System.Guid.NewGuid ().ToString ();
			displayCover (true);
			await AccountController.CheckAccount (token, email, password).ContinueWith (response => {

				Activity.RunOnUiThread( ()=> SetCheckAccountResult (email, password, response.Result));

			});
				
			return true;
		}

		void Logout(){

			ShowAlert (
				Activity.GetString (Resource.String.Global_Logout_Title),
				Activity.GetString (Resource.String.Global_Logout_Message), 
				Activity.GetString (Resource.String.Global_ButtonText_Logout),
				new System.EventHandler<DialogClickEventArgs> ((o, e) => {

					var dialog = o as global::Android.App.AlertDialog;
					Button btnClicked = dialog.GetButton(e.Which);
					if (btnClicked.Text == Activity.GetString (Resource.String.Global_ButtonText_Logout)) {
						CurrentUser = null;
						//txtProfileEmail.Text = string.Empty;
						((MainActivity)Activity).DoLogout();
					}
				}));
		}

	}
}

