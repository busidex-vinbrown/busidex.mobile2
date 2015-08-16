using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex")]			
	public class StartupActivity : BaseActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			//Remove title bar
			if(!isLoggedIn){
				this.RequestWindowFeature(global::Android.Views.WindowFeatures.NoTitle);					
			}
	
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.StartUp);


			var cookie = applicationResource.GetAuthCookie ();
			var loaderView = FindViewById (Resource.Id.fragment_holder);
			loaderView.Visibility = cookie == null ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;
			if (this.ActionBar != null) {
				this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			}

			var btnLogin = FindViewById<Button> (Resource.Id.btnConnect);
			var btnStart = FindViewById<Button> (Resource.Id.btnStart);

			btnLogin.Click += delegate {
				StartActivity(new Intent(this, typeof(LoginActivity)));
			};

			btnStart.Click += delegate {
				StartActivity(new Intent(this, typeof(ProfileActivity)));	
			};


		}
	}
}