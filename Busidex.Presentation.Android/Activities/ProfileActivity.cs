
using System;

using Android.App;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Profile")]			
	public class ProfileActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Profile);

			base.OnCreate (savedInstanceState);

			var txtProfileEmail = FindViewById<TextView> (Resource.Id.txtProfileEmail);

			var token = GetAuthCookie ();
			var accountJSON = Busidex.Mobile.AccountController.GetAccount (token);
			var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Busidex.Mobile.BusidexUser> (accountJSON);
			if(account != null){
				txtProfileEmail.Text = account.Email;
			}
		}
	}
}

