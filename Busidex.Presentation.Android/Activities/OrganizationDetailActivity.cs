
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
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "OrganizationDetailActivity")]			
	public class OrganizationDetailActivity : Activity
	{
		Organization organization;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Organization);

			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Organization");
			organization = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization> (data);

		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if(organization != null){
				Window.SetTitle(organization.Name);
			}
		}
	}
}

