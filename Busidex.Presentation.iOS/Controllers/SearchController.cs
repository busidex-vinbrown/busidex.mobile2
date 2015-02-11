

namespace Busidex.Presentation.iOS
{
	using System;
	using Foundation;
	using UIKit;
	using Busidex.Mobile;
	using Busidex.Mobile.Models;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using MessageUI;
	using GoogleAnalytics.iOS;

	partial class SearchController : BaseController
	{
		public static NSString cellID = new NSString ("cellId");

		public SearchController (IntPtr handle) : base (handle)
		{
			//TableView.RegisterClassForCell (typeof(SearchViewCell), cellID);
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Search");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwSearchResults.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);

			vwSearchResults.Hidden = true;
			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
				DoSearch ().Wait(new System.Threading.CancellationToken());
				vwSearchResults.Hidden = false;
				txtSearch.ResignFirstResponder (); // hide keyboard
			};

			txtSearch.CancelButtonClicked += delegate {
				txtSearch.ResignFirstResponder();
			};

			var height = NavigationController.NavigationBar.Frame.Size.Height;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			txtSearch.Frame = new CoreGraphics.CGRect (0, height, UIScreen.MainScreen.Bounds.Width, 52);
		}

		public override void ViewWillAppear (bool animated)
		{

			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		void GoToCard(){
			var cardController = Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)vwSearchResults.Source).SelectedCard;

			if (cardController != null) {
				NavigationController.PushViewController (cardController, true);
			}
		}

		void ShowPhoneNumbers(){
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)vwSearchResults.Source).SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNotes = false;
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No cards match your search";
			src.CardSelected += delegate {
				GoToCard();
			};

			src.SendingEmail += delegate(string email) {
				var _mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new []{email});
				_mailController.Finished += ( s, args) => args.Controller.DismissViewController (true, null);
				PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += url => UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace ("http://", "")));

			src.CardAddedToMyBusidex += AddCardToMyBusidexCache;

			src.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};

			src.SharingCard += delegate {
				ShareCard (((TableSource)vwSearchResults.Source).SelectedCard);
			};

			return src;
		}

		void LoadSearchResults(List<UserCard> cards){

			var src = ConfigureTableSourceEventHandlers(cards); 

			vwSearchResults.Source = src;
			vwSearchResults.ReloadData ();
			vwSearchResults.AllowsSelection = true;
			vwSearchResults.SetNeedsDisplay ();

			Overlay.Hide ();
		}

		protected override void StartSearch(){

			base.StartSearch ();

			var src = new TableSource (new List<UserCard>());

			vwSearchResults.Source = src;
			vwSearchResults.ReloadData ();
			vwSearchResults.AllowsSelection = true;
			vwSearchResults.SetNeedsDisplay ();

			View.SetNeedsDisplay ();
		}

		public new async Task<int> DoSearch(){

			var cookie = GetAuthCookie ();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
			}

			var ctrl = new Busidex.Mobile.SearchController ();
			ctrl.DoSearch (txtSearch.Text, token).ContinueWith(response => {

				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response.Result);
				var cards = new List<UserCard> ();
				float total = Search.SearchModel.Results.Count;
				float processed = 0;

				if (!Search.SearchModel.Results.Any ()) {
					InvokeOnMainThread (() => LoadSearchResults (new List<UserCard> ()));
				} else {
					foreach (var item in Search.SearchModel.Results) {
						if (item != null) {

							var imageUrl = Resources.THUMBNAIL_PATH + item.FrontFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.FrontFileName;

							var userCard = new UserCard ();
							userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
							userCard.Card = item;
							userCard.CardId = item.CardId;
							cards.Add (userCard);

							if (!File.Exists (Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + item.FrontFileName))) {
								Utils.DownloadImage (imageUrl, documentsPath, fName).ContinueWith (t => {
									processed++;
									if (processed.Equals(total)) {
										InvokeOnMainThread (() => LoadSearchResults (cards));
									} 
								});
							} else {
								processed++;
								if (processed.Equals(total)) {
									InvokeOnMainThread (() => LoadSearchResults (cards));
								}
							}
						}
					}
				}

			});
			return 1;
		}
	}
}
