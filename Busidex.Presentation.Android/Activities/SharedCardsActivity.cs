
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.IO;
using System.Collections.Generic;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "SharedCardsActivity")]			
	public class SharedCardsActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.SharedCardList);

			base.OnCreate (savedInstanceState);

			var path = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.SHARED_CARDS_FILE);
			LoadSharedCardsFromFile (path);

		}

		void LoadSharedCardsFromFile(string fullFilePath){
			if(File.Exists(fullFilePath)){
				var sharedCardsFile = File.OpenText (fullFilePath);
				var sharedCardsJson = sharedCardsFile.ReadToEnd ();
				ProcessSharedCards (sharedCardsJson);
			}
		}

		void ProcessSharedCards(string data){

			var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (data);
			var sharedCardList = new List<SharedCard> ();
			sharedCardList.AddRange (sharedCardResponse.SharedCards);

			var adapter = new SharedCardListAdapter (this, Resource.Id.lstCards, sharedCardList);

			var lstSharedCards = FindViewById<ListView> (Resource.Id.lstSharedCards); 

			lstSharedCards.Adapter = adapter;
		}

		void SaveSharedCard(SharedCard sharedCard){

			var cookie = GetAuthCookie ();
			// Accept/Decline the card
			var ctrl = new Busidex.Mobile.SharedCardController ();
			var cardId = new long? (sharedCard.Card.CardId);

			ctrl.UpdateSharedCards (
				sharedCard.Accepted.GetValueOrDefault() ?  cardId: null, 
				sharedCard.Declined.GetValueOrDefault() ? cardId : null, 
				cookie);

			// if the card was accepted, update local copy of MyBusidex
			if(sharedCard.Accepted.GetValueOrDefault()){

				var intent = new Intent(this, typeof(SharedCardsActivity));
				var data = Newtonsoft.Json.JsonConvert.SerializeObject(sharedCard);

				intent.PutExtra ("SharedCard", data);
				AddCardToMyBusidex (intent);

				// track the event
				ActivityController.SaveActivity ((long)EventSources.Add, sharedCard.Card.CardId, cookie);
			}

			// update local copy of Shared Cards
			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.SHARED_CARDS_FILE);
			if(File.Exists(fullFilePath)){
				var sharedCardsFile = File.OpenText (fullFilePath);
				var sharedCardsJson = sharedCardsFile.ReadToEnd ();

				var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsJson);
				sharedCardResponse.SharedCards.RemoveAll (c => c.Card.CardId == sharedCard.Card.CardId);

				sharedCardsJson = Newtonsoft.Json.JsonConvert.SerializeObject (sharedCardResponse);

				sharedCardsFile.Close ();

				Utils.SaveResponse (sharedCardsJson, Busidex.Mobile.Resources.SHARED_CARDS_FILE);
			}
		}
	}
}

