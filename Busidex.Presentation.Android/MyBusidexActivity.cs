
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Android.Content;
using System.Collections.Generic;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Busidex")]			
	public class MyBusidexActivity : BaseActivity
	{
		List<UserCard> Cards { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.MyBusidex);

			Cards = new List<UserCard> ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			LoadCardsFromFile (fullFilePath);

		}

		void ShowCard(Intent intent){

			Redirect(intent);
		}

		protected override void ProcessCards(string data){

			var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);
			myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);

			Cards = myBusidexResponse.MyBusidex.Busidex;

			var lstCards = FindViewById<ListView> (Resource.Id.lstCards);
			var adapter = new UserCardAdapter (this, Resource.Id.lstCards, myBusidexResponse.MyBusidex.Busidex);

			adapter.Redirect += ShowCard;

			lstCards.Adapter = adapter;
		}
	}
}

