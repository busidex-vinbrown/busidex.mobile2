

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views.Animations;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	public class SplashActivity : Activity
	{
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.Splash);

			var imgSplashLogo = FindViewById<ImageView> (Resource.Id.imgSplashLogo);
			var splashAnimation = AnimationUtils.LoadAnimation (this, Resource.Animation.Spin);
			var fadeOutAnimation = AnimationUtils.LoadAnimation (this, Resource.Animation.abc_fade_out);
			splashAnimation.AnimationEnd += delegate {
				UISubscriptionService.Init ();
				imgSplashLogo.StartAnimation(fadeOutAnimation);
			};
			fadeOutAnimation.AnimationEnd += delegate {
				
				imgSplashLogo.Visibility = Android.Views.ViewStates.Gone;
				Finish();
				StartActivity (typeof(MainActivity));
			};
			imgSplashLogo.StartAnimation (splashAnimation);


		}
	}
}

