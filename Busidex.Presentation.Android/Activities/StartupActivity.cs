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
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.StartUp);

			var btnLogin = FindViewById<Button> (Resource.Id.btnConnect);
			var btnStart = FindViewById<Button> (Resource.Id.btnStart);

			btnLogin.Click += delegate {
				StartActivity(new Intent(this, typeof(LoginActivity)));
			};

			btnStart.Click += delegate {
				StartActivity(new Intent(this, typeof(ProfileActivity)));	
			};

			RedirectToMainIfLoggedIn ();
		}
	}
}