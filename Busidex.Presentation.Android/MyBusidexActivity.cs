
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Busidex")]			
	public class MyBusidexActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.MyBusidex);

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			LoadCardsFromFile (fullFilePath);

		}

		protected override void ProcessCards(string data){

			var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);
			myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);

			var lstCards = FindViewById<ListView> (Resource.Id.lstCards);
			lstCards.Adapter = new UserCardAdapter (this, Resource.Id.lstCards, myBusidexResponse.MyBusidex.Busidex);
		}
	}
}

