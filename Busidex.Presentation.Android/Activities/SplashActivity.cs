using Android.App;
using Android.OS;
using Android.Content;

namespace Busidex.Presentation.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]			
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			StartNextActivity ();
		}

		private void StartNextActivity()
		{
			var intent = new Intent(this, typeof(StartupActivity));
			intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearWhenTaskReset);
			StartActivity(intent);
			Finish();
		}
	}
}