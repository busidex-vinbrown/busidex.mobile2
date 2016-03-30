
using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Views.Animations;
using Busidex.Mobile.Models;
using Android.Views.InputMethods;
using Android.App;

namespace Busidex.Presentation.Droid.v2
{
	public class LoginFragment : GenericViewPagerFragment
	{
		TextView txtLoginFailed { get; set; }
		TextView txtUserName { get; set; }
		TextView txtPassword { get; set; }
		ImageView imgLogo { get; set; }
	
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.Login, container, false);

			txtLoginFailed = view.FindViewById<TextView> (Resource.Id.txtLoginFailed);
			txtUserName = view.FindViewById<TextView> (Resource.Id.txtUserName);
			txtPassword = view.FindViewById<TextView> (Resource.Id.txtPassword);

			imgLogo = view.FindViewById<ImageView> (Resource.Id.imgLogo);

			txtLoginFailed.Visibility = ViewStates.Gone;

			var button = view.FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += async delegate {
				await DoLogin().ContinueWith(response => {
					if (!string.IsNullOrEmpty (response.Result) && !response.Result.Contains ("404")) {
						var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response.Result);
						if(loginResponse.UserId > 0){
							var userId = loginResponse != null ? loginResponse.UserId : 0;
							BaseApplicationResource.SetAuthCookie (userId);

							Activity.RunOnUiThread (() => {
								imgLogo.ClearAnimation ();
								((MainActivity)Activity).LoginComplete ();
							});
						}else{
							LoginFailed();
						}

					} else {
						LoginFailed();
					}
				});
			};

			txtUserName.TextChanged += (sender, e) => HideErrorMessage();
			txtPassword.TextChanged += (sender, e) => HideErrorMessage();
			return view;
		}

		void LoginFailed(){
			Activity.RunOnUiThread (() => {
				txtLoginFailed.Visibility = ViewStates.Visible;
				imgLogo.ClearAnimation ();
			});
		}

		void HideErrorMessage(){
			txtLoginFailed.Visibility = ViewStates.Gone;
		}

		async Task<string> DoLogin(){

			var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(txtPassword.WindowToken, 0);

			var userName = txtUserName.Text;
			var password = txtPassword.Text;

			var rotateAboutCornerAnimation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.Rotate);
			rotateAboutCornerAnimation.RepeatMode = RepeatMode.Reverse;
			rotateAboutCornerAnimation.RepeatCount = 10;
			imgLogo.StartAnimation (rotateAboutCornerAnimation);
			var loginController = new Busidex.Mobile.LoginController ();

			return await loginController.DoLogin (userName, password);
		}
	}
}

