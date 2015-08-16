
using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using System.Linq;
using Android.InputMethodServices;
using Android.Content;
using Android.Views.InputMethods;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "EventCardsActivity")]			
	public class EventCardsActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }
		static UserCardAdapter EventCardsAdapter { get; set; }
		static SearchView txtFilterEventCards { get; set; }
		Activity context { get; set; }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.EventCards);

			var data = Intent.GetStringExtra ("Event");
			var tag = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTag> (data);
			Title = tag.Description;

			//TrackAnalyticsEvent (
			//Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, 
			//Busidex.Mobile.Resources.GA_LABEL_EVENT, 
			//string.Format(Busidex.Mobile.Resources.GA_LABEL_EVENT_NAME, Title), 0);

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text));
			//LoadFromFile (fullFilePath);
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				EventCardsAdapter.Filter.InvokeFilter ("");
			}else{
				EventCardsAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override void ProcessFile(string data){

			var eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (data);

			Cards = new List<UserCard> ();

			foreach (var card in eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList()) {
				if (card != null) {

					var userCard = new UserCard (card);

					userCard.ExistsInMyBusidex = card.ExistsInMyBusidex;
					userCard.Card = card;
					userCard.CardId = card.CardId;

					Cards.Add (userCard);
				}
			}

			txtFilterEventCards = FindViewById<SearchView> (Resource.Id.txtFilterEventCards);

			var lstEventCards = FindViewById<ListView> (Resource.Id.lstEventCards);
			EventCardsAdapter = new UserCardAdapter (this, Resource.Id.lstCards, Cards);

//			EventCardsAdapter.Redirect += ShowCard;
//			EventCardsAdapter.SendEmail += SendEmail;
//			EventCardsAdapter.OpenBrowser += OpenBrowser;
//			EventCardsAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
//			EventCardsAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
//			EventCardsAdapter.OpenMap += OpenMap;

			EventCardsAdapter.ShowNotes = false;

			lstEventCards.Adapter = EventCardsAdapter;

			txtFilterEventCards.QueryTextChange += delegate {
				DoFilter(txtFilterEventCards.Query);
			};

			txtFilterEventCards.Iconified = false;
			lstEventCards.RequestFocus (global::Android.Views.FocusSearchDirection.Down);
			//DismissKeyboard (txtFilterEventCards.WindowToken);

			txtFilterEventCards.Touch += delegate {
				txtFilterEventCards.Focusable = true;
				txtFilterEventCards.RequestFocus();
			};
		}

	}
}

