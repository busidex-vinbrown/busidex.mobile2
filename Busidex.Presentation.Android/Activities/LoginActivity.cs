
using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Content;
using System.Threading.Tasks;
using Android.Animation;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Sign In")]			
	public class LoginActivity : BaseActivity
	{
		TextView txtLoginFailed { get; set; }
		TextView txtUserName { get; set; }
		TextView txtPassword { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Login);

			base.OnCreate (savedInstanceState);

			txtLoginFailed = FindViewById<TextView> (Resource.Id.txtLoginFailed);
			txtUserName = FindViewById<TextView> (Resource.Id.txtUserName);
			txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);

			txtLoginFailed.Visibility = global::Android.Views.ViewStates.Gone;



			var button = FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				DoLogin();
			};

			txtUserName.TextChanged += (sender, e) => HideErrorMessage();
			txtPassword.TextChanged += (sender, e) => HideErrorMessage();
		}

		void HideErrorMessage(){
			txtLoginFailed.Visibility = global::Android.Views.ViewStates.Gone;
		}

		async Task<bool> DoLogin(){

			var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(txtPassword.WindowToken, 0);

			var userName = txtUserName.Text;
			var password = txtPassword.Text;
			var imgLogo = FindViewById<ImageView> (Resource.Id.imgLogo);
		
			var rotateAboutCornerAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.Rotate);
			rotateAboutCornerAnimation.RepeatMode = RepeatMode.Reverse;
			rotateAboutCornerAnimation.RepeatCount = 10;
			imgLogo.StartAnimation (rotateAboutCornerAnimation);

			await Busidex.Mobile.LoginController.DoLogin(userName, password).ContinueWith(async response => {
				if (!string.IsNullOrEmpty(response.Result) && !response.Result.Contains ("404")) {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);
					var userId = loginResponse != null ? loginResponse.UserId : 0;
					applicationResource.SetAuthCookie (userId);

					RunOnUiThread (() => {
						imgLogo.ClearAnimation ();
						RedirectToMainIfLoggedIn ();
					});
						
				}else{
					RunOnUiThread (() => {
						txtLoginFailed.Visibility = global::Android.Views.ViewStates.Visible;
						imgLogo.ClearAnimation ();
					});
				}
			});
			return true;
		}


	}
}

