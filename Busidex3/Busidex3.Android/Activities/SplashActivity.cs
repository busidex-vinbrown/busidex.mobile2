using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.Animations;
using Android.Widget;

namespace Busidex3.Droid.Activities
{
	[Activity (Theme = "@style/Theme.Splash",
		MainLauncher = true,
		NoHistory = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	[IntentFilter (new [] { Intent.ActionView },
		DataScheme = "busidex",
		DataPathPrefix = "/Uebo",
		Categories = new [] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Splash);
		}

        protected override void OnResume()
        {
            base.OnResume();
            var imgSplashLogo = FindViewById<ImageView>(Resource.Id.imgSplashLogo);
            var splashAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.Spin);
            splashAnimation.AnimationEnd += delegate {

                var mainActivityIntent = new Intent(this, typeof(MainActivity));
                StartActivity(mainActivityIntent);
                Finish();
            };

            imgSplashLogo.StartAnimation(splashAnimation);
        }
    }
}
