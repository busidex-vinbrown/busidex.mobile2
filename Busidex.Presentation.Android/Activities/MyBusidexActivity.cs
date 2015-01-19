
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using System.Collections.Generic;
using Android.Content;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "My Busidex")]			
	public class MyBusidexActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }
		static UserCardAdapter MyBusidexAdapter { get; set; }
		static EditText txtFilter { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.MyBusidex);

			Cards = new List<UserCard> ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			LoadFromFile (fullFilePath);
		}

		public override void OnBackPressed ()
		{
			Redirect(new Intent(this, typeof(MainActivity)));
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				MyBusidexAdapter.Filter.InvokeFilter ("");
			}else{
				MyBusidexAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override void ProcessFile(string data){

			var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);
			myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);

			Cards = myBusidexResponse.MyBusidex.Busidex;

			var lstCards = FindViewById<ListView> (Resource.Id.lstCards);
			txtFilter = FindViewById<EditText> (Resource.Id.txtFilter);

			var txtFilter1 = FindViewById<SearchView> (Resource.Id.txtFilter1);
			MyBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, myBusidexResponse.MyBusidex.Busidex);

			var lblNoCardsMessage = FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
			lblNoCardsMessage.Text = GetString (Resource.String.MyBusidex_NoCards);

			lblNoCardsMessage.Visibility = Cards.Count == 0 ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;

			MyBusidexAdapter.Redirect += ShowCard;
			MyBusidexAdapter.SendEmail += SendEmail;
			MyBusidexAdapter.OpenBrowser += OpenBrowser;
			MyBusidexAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			MyBusidexAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			MyBusidexAdapter.OpenMap += OpenMap;

			MyBusidexAdapter.ShowNotes = true;

			lstCards.Adapter = MyBusidexAdapter;

			txtFilter1.QueryTextChange += delegate {
				DoFilter(txtFilter1.Query);
			};

			txtFilter1.Touch += delegate {
				txtFilter.Focusable = true;
				txtFilter.RequestFocus();
			};
				
			txtFilter.Visibility = global::Android.Views.ViewStates.Gone;
			txtFilter.TextChanged += (s, e) => DoFilter (txtFilter.Text);
		}
	}
}

