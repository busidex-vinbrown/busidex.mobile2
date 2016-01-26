
using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class SharedCardListController : BaseCardViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public SharedCardListController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			base.TableView = vwSharedCards;

			vwSharedCards.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			LoadSharedCards ();

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Shared Card List");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.NavigationBar.SetBackgroundImage (null, UIBarMetrics.Default);
			}
		}

		void LoadSharedCardsFromFile(string fullFilePath){
			if(File.Exists(fullFilePath)){
				using (var sharedCardsFile = File.OpenText (fullFilePath)) {
					var sharedCardsJson = sharedCardsFile.ReadToEnd ();
					ProcessSharedCards (sharedCardsJson);
				}
			}
		}

		void ProcessSharedCards(string data){

			var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (data);
			var sharedCardList = new List<SharedCard> ();
			sharedCardList.AddRange (sharedCardResponse.SharedCards);

			vwSharedCards.Source = ConfigureTableSourceEventHandlers(sharedCardList);

			Badge.Plugin.CrossBadge.Current.SetBadge (sharedCardList.Count);	
		}

		void LoadSharedCards(){

			var fullFilePath = Path.Combine (documentsPath, Resources.SHARED_CARDS_FILE);
			if (File.Exists (fullFilePath)) {
				LoadSharedCardsFromFile (fullFilePath);
			} else {
				var controller = new DataViewController ();
				controller.GetNotifications ();
			}
		}

		void SaveSharedCard(SharedCard sharedCard){

			var cookie = GetAuthCookie ();
			// Accept/Decline the card
			var ctrl = new Busidex.Mobile.SharedCardController ();
			var cardId = new long? (sharedCard.Card.CardId);

			ctrl.UpdateSharedCards (
				sharedCard.Accepted.GetValueOrDefault() ?  cardId: null, 
				sharedCard.Declined.GetValueOrDefault() ? cardId : null, 
				cookie.Value);

			// if the card was accepted, update local copy of MyBusidex
			if(sharedCard.Accepted.GetValueOrDefault()){
				var newCard = new UserCard {
					Card = sharedCard.Card,
					CardId = sharedCard.Card.CardId
				};
				AddCardToMyBusidexCache (newCard);

				// track the event
				ActivityController.SaveActivity ((long)EventSources.Add, sharedCard.Card.CardId, cookie.Value);
			}

			// update local copy of Shared Cards
			var fullFilePath = Path.Combine (documentsPath, Resources.SHARED_CARDS_FILE);
			if(File.Exists(fullFilePath)){
				var sharedCardsFile = File.OpenText (fullFilePath);
				var sharedCardsJson = sharedCardsFile.ReadToEnd ();

				var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsJson);
				sharedCardResponse.SharedCards.RemoveAll (c => c.Card.CardId == sharedCard.Card.CardId);

				sharedCardsJson = Newtonsoft.Json.JsonConvert.SerializeObject (sharedCardResponse);

				sharedCardsFile.Close ();

				Utils.SaveResponse (sharedCardsJson, Resources.SHARED_CARDS_FILE);
			}
		}

		SharedCardTableSource ConfigureTableSourceEventHandlers(List<SharedCard> sharedCards){

			var src = new SharedCardTableSource (sharedCards);
			src.NoCardsMessage = "There are no more shared cards to load";

			src.CardSelected += delegate {
				GoToCard();
			};
			src.CardShared += SaveSharedCard;

			return src;
		}
	}
}

