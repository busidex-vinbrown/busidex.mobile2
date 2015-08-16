using Android.App;
using Android.OS;

namespace Busidex.Presentation.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]			
	public class SplashActivity : BaseActivity
	{


		protected override void OnStart ()
		{
			base.OnStart ();
			RedirectToMainIfLoggedIn ();
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.StartUp);
		}
	}
}