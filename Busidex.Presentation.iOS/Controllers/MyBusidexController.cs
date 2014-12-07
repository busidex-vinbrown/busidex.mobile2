using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MessageUI;

namespace Busidex.Presentation.iOS
{
	partial class MyBusidexController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		List<UserCard> FilterResults;
		const string NO_CARDS = "You Don't Have Any Cards In Your Collection. Search for some and add them!";
		MFMailComposeViewController _mailController;

		public MyBusidexController (IntPtr handle) : base (handle)
		{
		}

		void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			FilterResults.AddRange (
				Application.MyBusidex.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant().Contains (loweredFilter)) ||
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter)))
				));

			TableSource src = ConfigureTableSourceEventHandlers(FilterResults);
			src.IsFiltering = true;
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		void ResetFilter(){

			SearchBar.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Application.MyBusidex);
			src.NoCardsMessage = NO_CARDS;
			src.IsFiltering = false;
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNotes = true;
			src.ShowNoCardMessage = true;
			src.CardSelected += delegate {
				GoToCard();
			};

			src.EditingNotes += delegate {
				EditNotes();
			};	

			src.SendingEmail += delegate(string email) {

				_mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new []{email});

				_mailController.Finished += ( s, args) => InvokeOnMainThread (
					() => args.Controller.DismissViewController (true, null)
				);
				PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += url => UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace ("http://", "")));

			src.CardAddedToMyBusidex += AddCardToMyBusidex;

			src.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};
			return src;
		}

		void ConfigureSearchBar(){

			SearchBar.Placeholder = "Filter";
			SearchBar.BarStyle = UIBarStyle.Default;
			SearchBar.ShowsCancelButton = true;

			SearchBar.SearchButtonClicked += delegate {
				SetFilter(SearchBar.Text);
				SearchBar.ResignFirstResponder();
			};
			SearchBar.CancelButtonClicked += delegate {
				ResetFilter();
				SearchBar.ResignFirstResponder();
			};
		}

		void GoToCard(){
			var cardController = Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)TableView.Source).SelectedCard;

			if (cardController != null) {
				NavigationController.PushViewController (cardController, true);
			}
		}

		void EditNotes(){

			var notesController = Storyboard.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)TableView.Source).SelectedCard;

			if (notesController != null) {
				NavigationController.PushViewController (notesController, true);
			}
		}

		void ShowPhoneNumbers(){
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)TableView.Source).SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}
		}

		protected override void ProcessCards(string data){
			MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);

			Application.MyBusidex = new List<UserCard> ();
			myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);
			Application.MyBusidex.AddRange (myBusidexResponse.MyBusidex.Busidex.Where (c => c.Card != null));

			if (TableView.Source == null) {
				var src = ConfigureTableSourceEventHandlers(Application.MyBusidex);
				src.NoCardsMessage = NO_CARDS;
				TableView.Source = src;
			}
			TableView.AllowsSelection = true;
		}

		void SaveMyBusidexResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			File.WriteAllText (fullFilePath, response);
		}

		void LoadMyBusidexAsync(){
			var cookie = GetAuthCookie ();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);

				if(!string.IsNullOrEmpty(response.Result)){
					ProcessCards (response.Result);
					SaveMyBusidexResponse (response.Result);
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		public void LoadMyBusidex(){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath)) {
				LoadCardsFromFile (fullFilePath);
			} else {
				LoadMyBusidexAsync ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureSearchBar ();

			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyBusidex ();
		}
	}
}
