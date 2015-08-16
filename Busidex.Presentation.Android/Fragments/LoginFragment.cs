
using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Views.Animations;
using Busidex.Mobile.Models;
using Android.Views.InputMethods;

namespace Busidex.Presentation.Android
{
	public class LoginFragment : BaseFragment
	{
		TextView txtLoginFailed { get; set; }
		TextView txtUserName { get; set; }
		TextView txtPassword { get; set; }
		ImageView imgLogo { get; set; }
	

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.Login, container, false);

			txtLoginFailed = view.FindViewById<TextView> (Resource.Id.txtLoginFailed);
			txtUserName = view.FindViewById<TextView> (Resource.Id.txtUserName);
			txtPassword = view.FindViewById<TextView> (Resource.Id.txtPassword);

			imgLogo = view.FindViewById<ImageView> (Resource.Id.imgLogo);

			txtLoginFailed.Visibility = ViewStates.Gone;



			var button = view.FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				DoLogin();
			};

			txtUserName.TextChanged += (sender, e) => HideErrorMessage();
			txtPassword.TextChanged += (sender, e) => HideErrorMessage();
			return view;
		}

		void HideErrorMessage(){
			txtLoginFailed.Visibility = ViewStates.Gone;
		}

		async Task<bool> DoLogin(){

			var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(txtPassword.WindowToken, 0);

			var userName = txtUserName.Text;
			var password = txtPassword.Text;

			var rotateAboutCornerAnimation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.Rotate);
			rotateAboutCornerAnimation.RepeatMode = RepeatMode.Reverse;
			rotateAboutCornerAnimation.RepeatCount = 10;
			imgLogo.StartAnimation (rotateAboutCornerAnimation);

			await Busidex.Mobile.LoginController.DoLogin(userName, password).ContinueWith(async response => {
				if (!string.IsNullOrEmpty(response.Result) && !response.Result.Contains ("404")) {
					var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);
					var userId = loginResponse != null ? loginResponse.UserId : 0;
					applicationResource.SetAuthCookie (userId);

					Activity.RunOnUiThread (() => {
						imgLogo.ClearAnimation ();
						((SplashActivity)Activity).LoadFragment(new MainFragment());
					});

				}else{
					Activity.RunOnUiThread (() => {
						txtLoginFailed.Visibility = ViewStates.Visible;
						imgLogo.ClearAnimation ();
					});
				}
			});
			return true;
		}
	}
}

