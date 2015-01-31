
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
		static SearchView txtFilter { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);

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
			txtFilter = FindViewById<SearchView> (Resource.Id.txtFilter);

			MyBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, myBusidexResponse.MyBusidex.Busidex);

			var lblNoCardsMessage = FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
			lblNoCardsMessage.Text = GetString (Resource.String.MyBusidex_NoCards);

			lblNoCardsMessage.Visibility = Cards.Count == 0 ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
			txtFilter.Visibility = Cards.Count == 0 ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;

			MyBusidexAdapter.Redirect += ShowCard;
			MyBusidexAdapter.SendEmail += SendEmail;
			MyBusidexAdapter.OpenBrowser += OpenBrowser;
			MyBusidexAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			MyBusidexAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			MyBusidexAdapter.OpenMap += OpenMap;

			MyBusidexAdapter.ShowNotes = true;

			lstCards.Adapter = MyBusidexAdapter;

			txtFilter.QueryTextChange += delegate {
				DoFilter(txtFilter.Query);
			};

			txtFilter.Iconified = false;

			txtFilter.Touch += delegate {
				txtFilter.Focusable = true;
				txtFilter.RequestFocus();
			};
				
		}
	}
}

