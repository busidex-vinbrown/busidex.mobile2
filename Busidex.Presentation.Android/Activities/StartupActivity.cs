﻿

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex", MainLauncher = true)]			
	public class StartupActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.StartUp);

			var btnLogin = FindViewById<Button> (Resource.Id.btnConnect);

			btnLogin.Click += delegate {
				var intent = new Intent(this, typeof(LoginActivity));
				StartActivity(intent);
			};

			RedirectToMainIfLoggedIn ();
		}


	}
}
