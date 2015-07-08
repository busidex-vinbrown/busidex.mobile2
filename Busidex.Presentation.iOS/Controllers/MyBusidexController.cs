using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MessageUI;
using GoogleAnalytics.iOS;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	partial class MyBusidexController : BaseCardViewController
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
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter))) ||
					(c.Card.Tags != null && c.Card.Tags.Any(t => t.Text.ToLowerInvariant().Contains(loweredFilter)))
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

			src.CardAddedToMyBusidex += AddCardToMyBusidexCache;

			src.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};

			src.SharingCard += delegate {
				ShareCard (((TableSource)TableView.Source).SelectedCard);
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
			SearchBar.TextChanged += delegate {
				if(SearchBar.Text.Length == 0){
					ResetFilter();
					SearchBar.ResignFirstResponder();
				}
			};
		}

		void EditNotes(){

			var notesController = Storyboard.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)TableView.Source).SelectedCard;

			if (notesController != null) {
				NavigationController.PushViewController (notesController, true);
			}

			string name = Resources.GA_LABEL_NOTES;
			if(notesController.UserCard != null && notesController.UserCard.Card != null){
				name = string.IsNullOrEmpty(notesController.UserCard.Card.Name) ? notesController.UserCard.Card.CompanyName : notesController.UserCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_NOTES, name, 0);
		}

		void ShowPhoneNumbers(){
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)TableView.Source).SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}

			string name = Resources.GA_LABEL_PHONE;
			if(phoneViewController.UserCard != null && phoneViewController.UserCard.Card != null){
				name = string.IsNullOrEmpty(phoneViewController.UserCard.Card.Name) ? phoneViewController.UserCard.Card.CompanyName : phoneViewController.UserCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_PHONE, name, 0);
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
			
		void LoadMyBusidexAsync(){
			var cookie = GetAuthCookie ();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				ctrl.GetMyBusidex (cookie.Value).ContinueWith(response => {
					if(!string.IsNullOrEmpty(response.Result)){
						ProcessCards (response.Result);
						Utils.SaveResponse (response.Result, Resources.MY_BUSIDEX_FILE);
					}
				});
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

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "My Busidex");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_MY_BUSIDEX_LABEL, Resources.GA_LABEL_LIST, 0);

			ConfigureSearchBar ();

			base.TableView = TableView;

			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyBusidex ();

			var height = NavigationController.NavigationBar.Frame.Size.Height;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			SearchBar.Frame = new CGRect (0, height, UIScreen.MainScreen.Bounds.Width, 52);
		}
	}
}
