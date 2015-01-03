
using System;

using Android.App;
using Android.OS;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "BaseCardActivity")]			
	public class BaseCardActivity : BaseActivity
	{
		protected UserCard UserCard { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Card");
			if (!string.IsNullOrEmpty (data)) {
				UserCard = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data);
			}
		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if(UserCard != null){
				Window.SetTitle(UserCard.Card.Name);
			}
		}
	}
}

