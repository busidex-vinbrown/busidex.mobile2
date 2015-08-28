
using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Views.Animations;
using System.Threading.Tasks;
using Android.App;

namespace Busidex.Presentation.Android
{
	public class ProfileFragment : BaseFragment, GestureDetector.IOnGestureListener, View.IOnTouchListener
	{

		ImageView imgProfileEmailSaved;
		ImageView imgProfilePasswordSaved;
		TextView lblEmailError;
		TextView lblPasswordError;
		//bool showPassword = true;


		#region Touch Events
		public override bool OnTouch (View v, MotionEvent e)
		{
			_detector.OnTouchEvent (e);
			return true;
		}

		public override bool OnDown (MotionEvent e)
		{
			return true;
		}

		public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			const float SWIPE_THRESHOLD = 400;
			if (e1.GetX () - e2.GetX () > SWIPE_THRESHOLD) {
				var mainFragment = ((SplashActivity)Activity).fragments[typeof(MainFragment).Name];
				((SplashActivity)Activity).LoadFragment (
					mainFragment,
					Resource.Animator.SlideAnimationFast, 
					Resource.Animator.SlideOutAnimationFast);
			}

			return true;
		}

		public override void OnLongPress (MotionEvent e)
		{

		}

		public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

		public override void OnShowPress (MotionEvent e)
		{

		}

		public override bool OnSingleTapUp (MotionEvent e)
		{
			return false;
		}


		#endregion

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
		
			// FIXME: need to do less in this methhod. view takes too long to load

			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			View profileView = inflater.Inflate(Resource.Layout.Profile, container, false);

			profileView.SetOnTouchListener( this );

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

				//showPassword = false;
				txtProfileDescription.SetText (Resource.String.Profile_DescriptionUpdateAccount);

				btnSaveProfile.Click += async delegate {
					await UpdateEmail (token, txtProfileEmail.Text);
				};
			}

			return profileView;
		}

		bool SetEmailChangedResult(string result)
		{

			if (result.IndexOf ("400", StringComparison.Ordinal) >= 0) {
				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
				lblEmailError.Visibility = ViewStates.Visible;
				lblEmailError.SetText (Resource.String.Profile_ErrorEmailGeneral);
				return false;
			}

			if (result.ToLowerInvariant ().IndexOf ("email updated", StringComparison.Ordinal) >= 0) {
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

//		void SetCheckAccountResult(string email, string password, string result){
//			const string ERROR_UNABLE_TO_CREATE_ACCOUNT = "unable to create new account:";
//			var oResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAccountResult> (result);
//
//			if (result.ToLowerInvariant ().IndexOf (ERROR_UNABLE_TO_CREATE_ACCOUNT, StringComparison.Ordinal) >= 0) {
//				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
//				lblEmailError.Visibility = ViewStates.Visible;
//				lblEmailError.Text = result.Replace (ERROR_UNABLE_TO_CREATE_ACCOUNT, string.Empty);
//			}else if (result.ToLowerInvariant ().IndexOf (GetString(Resource.String.Profile_ErrorAccountExists), StringComparison.Ordinal) >= 0) {
//				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
//				lblEmailError.Visibility = ViewStates.Visible;
//				lblEmailError.Text = "This email is already in use";
//			} else if (oResult != null && oResult.Success) {
//				imgProfileEmailSaved.Visibility = ViewStates.Visible;
//				lblEmailError.Visibility = ViewStates.Invisible;
//				if (showPassword) {
//					imgProfilePasswordSaved.Visibility = ViewStates.Visible;
//					lblPasswordError.Visibility = ViewStates.Invisible;
//				}
//				LoginController.DoLogin(email, password).ContinueWith(response => {
//					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);
//
//					var userId = loginResponse != null ? loginResponse.UserId : 0;
//
//					applicationResource.SetAuthCookie (userId);
//
//					Activity.RunOnUiThread (RedirectToMainIfLoggedIn);	
//				});
//			} else {
//				imgProfileEmailSaved.Visibility = ViewStates.Invisible;
//				lblEmailError.Visibility = ViewStates.Visible;
//				lblEmailError.Text = GetString (Resource.String.Profile_ErrorAccountGeneral);
//			}
//		}

		async Task<bool> UpdateEmail(string token, string email){
			 await SettingsController.ChangeEmail (email, token).ContinueWith(response => {

				Activity.RunOnUiThread (() => {
					if(SetEmailChangedResult (response.Result)){
						var slideOut = AnimationUtils.LoadAnimation (Activity, Resource.Animation.SlideOutAnimationFast);
						View.Visibility = ViewStates.Gone;
						View.StartAnimation(slideOut);
						((MainActivity)Activity).profileIsOpen = false;
						((MainActivity)Activity).interceptTouchEvents = true;
					}
				});
			});
			return true;
		}

//		async Task<bool> CheckAccount(string token, string email, string password){
//			token = Guid.NewGuid ().ToString ();
//			await AccountController.CheckAccount (token, email, password).ContinueWith (response => {
//
//				Activity.RunOnUiThread( ()=> SetCheckAccountResult (email, password, response.Result));
//
//			});
//				
//			return true;
//		}


	}
}

