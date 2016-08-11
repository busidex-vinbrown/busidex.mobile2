using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views.Animations;
using Busidex.Mobile;
using System.Threading.Tasks;
using Android.Content;

namespace Busidex.Presentation.Droid.v2
{
	[Activity (Theme = "@style/Theme.Splash",
		MainLauncher = true,
		NoHistory = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (Mobile.Resources.XAMARIN_INSIGHTS_KEY, this);
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Splash);

			Task.Factory.StartNew (() => {
				BaseApplicationResource.Init (this);
				UISubscriptionService.AuthToken = BaseApplicationResource.GetAuthCookie ();
				UISubscriptionService.Init ();
			});
			Task.Factory.StartNew (() => {
				Xamarin.Insights.Initialize (GetString (Resource.String.InsightsApiKey), ApplicationContext);

			});

			var imgSplashLogo = FindViewById<ImageView> (Resource.Id.imgSplashLogo);
			var splashAnimation = AnimationUtils.LoadAnimation (this, Resource.Animation.Spin);
			splashAnimation.AnimationEnd += delegate {

				var mainActivityIntent = new Intent (this, typeof (MainActivity));
				StartActivity (mainActivityIntent);
				Finish ();
			};
	
			imgSplashLogo.StartAnimation (splashAnimation);
		}
	}
}
