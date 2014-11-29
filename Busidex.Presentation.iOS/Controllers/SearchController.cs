

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

	partial class SearchController : BaseController
	{
		public static NSString cellID = new NSString ("cellId");

		public SearchController (IntPtr handle) : base (handle)
		{
			//TableView.RegisterClassForCell (typeof(SearchViewCell), cellID);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwSearchResults.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);


			vwSearchResults.Hidden = true;
			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
				DoSearch ();
				vwSearchResults.Hidden = false;
				txtSearch.ResignFirstResponder (); // hide keyboard
			};

			txtSearch.CancelButtonClicked += delegate {
				txtSearch.ResignFirstResponder();
			};
		}

		public override void ViewWillAppear (bool animated)
		{

			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		private void GoToCard(){
			var cardController = this.Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)this.vwSearchResults.Source).SelectedCard;

			if (cardController != null) {
				this.NavigationController.PushViewController (cardController, true);
			}
		}

		private void ShowPhoneNumbers(){
			var phoneViewController = this.Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)this.vwSearchResults.Source).SelectedCard;

			if (phoneViewController != null) {
				this.NavigationController.PushViewController (phoneViewController, true);
			}
		}

		private TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNotes = false;
			src.ShowNoCardMessage = data.Count() == 0;
			src.NoCardsMessage = "No cards match your search";
			src.CardSelected += delegate {
				GoToCard();
			};

			src.SendingEmail += delegate(string email) {
				MFMailComposeViewController _mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new string[]{email});
				_mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
					args.Controller.DismissViewController (true, null);
				};
				this.PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += delegate(string url) {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace("http://", "")));
			};

			src.CardAddedToMyBusidex += new CardAddedToMyBusidexHandler (AddCardToMyBusidex);

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};
			return src;
		}

		private void LoadSearchResults(List<UserCard> cards){

			var src = ConfigureTableSourceEventHandlers(cards); 

			this.vwSearchResults.Source = src;
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			Overlay.Hide ();
		}

		protected override void StartSearch(){

			base.StartSearch ();

			var src = new TableSource (new List<UserCard>());

			this.vwSearchResults.Source = src;
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			this.View.SetNeedsDisplay ();
		}

		public new async Task<int> DoSearch(){

			var cookie = GetAuthCookie ();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
			}

			var ctrl = new Busidex.Mobile.SearchController ();
			string response = await ctrl.DoSearch (txtSearch.Text, token);

			if (!string.IsNullOrEmpty (response)) {

				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response);
				List<UserCard> cards = new List<UserCard> ();
				float total = Search.SearchModel.Results.Count;
				float processed = 0;

				if (Search.SearchModel.Results.Count () == 0) {
					LoadSearchResults (new List<UserCard> ());
				} else {
					foreach (var item in Search.SearchModel.Results) {
						if (item != null) {

							var imagePath = Busidex.Mobile.Utils.CARD_PATH + item.FrontFileId + "." + item.FrontType;
							var fName = item.FrontFileId + "." + item.FrontType;

							var userCard = new UserCard ();
							userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
							userCard.Card = item;
							userCard.CardId = item.CardId;
							cards.Add (userCard);

							if (!File.Exists (System.IO.Path.Combine (documentsPath, item.FrontFileId + "." + item.FrontType))) {
								await Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

									if (++processed == total) {

										this.InvokeOnMainThread (() => {
											LoadSearchResults (cards);
										});

									} 
								});
							} else {

								if (++processed == total) {
									LoadSearchResults (cards);
								}
							}
						}
					}
				}
			}
			return 1;
		}
	}
}
