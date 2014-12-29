
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "StartupActivity", MainLauncher = true)]			
	public class StartupActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.StartUp);

			var btnLogin = FindViewById<Button> (Resource.Id.btnConnect);

			btnLogin.Click += delegate {
				SetContentView (Resource.Layout.Login);
			};
		}
	}
}

