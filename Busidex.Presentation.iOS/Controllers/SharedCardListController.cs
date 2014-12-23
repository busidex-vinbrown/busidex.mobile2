
using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class SharedCardListController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public SharedCardListController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadSharedCards ();

			vwSharedCards.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			// Perform any additional setup after loading the view, typically from a nib.
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
				var sharedCardsFile = File.OpenText (fullFilePath);
				var sharedCardsJson = sharedCardsFile.ReadToEnd ();
				ProcessSharedCards (sharedCardsJson);
			}
		}

		void ProcessSharedCards(string data){

			var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (data);
			var sharedCardList = new List<SharedCard> ();
			sharedCardList.AddRange (sharedCardResponse.SharedCards);

			vwSharedCards.Source = ConfigureTableSourceEventHandlers(sharedCardList);
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

		void GoToCard(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

			var cardController = board.InstantiateViewController ("CardViewController") as CardViewController;

			cardController.UserCard = ((SharedCardTableSource)vwSharedCards.Source).SelectedCard;

			if (cardController != null) {
				NavigationController.PushViewController (cardController, true);
			}
		}

		SharedCardTableSource ConfigureTableSourceEventHandlers(List<SharedCard> sharedCards){

			var src = new SharedCardTableSource (sharedCards);
			src.NoCardsMessage = "There are no more shared cards to load";

			src.CardSelected += delegate {
				GoToCard();
			};

			return src;
		}
	}
}

