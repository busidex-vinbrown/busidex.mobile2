
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Organizations")]			
	public class MyOrganizationsActivity : BaseActivity
	{
		List<Organization> Organizations { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.MyOrganizations);

			Organizations = new List<Organization> ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_ORGANIZATIONS_FILE);
			LoadFromFile (fullFilePath);
		}

		protected override void ProcessFile(string data){

			var myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (data);

			Organizations = myOrganizationsResponse.Model;

			if (Organizations != null) {
			
				var lstOrganizations = FindViewById<ListView> (Resource.Id.lstOrganizations);
				var adapter = new OrganizationAdapter (this, Resource.Id.lstCards, Organizations);

				lstOrganizations.Adapter = adapter;
			}else{
				Toast.MakeText (this, Resource.String.Organization_NoOrganizations, ToastLength.Long);
			}
		}

		public override void OnBackPressed ()
		{
			Redirect(new Intent(this, typeof(MainActivity)));
		}
	}
}

