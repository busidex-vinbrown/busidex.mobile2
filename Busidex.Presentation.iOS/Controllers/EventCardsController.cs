
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

namespace Busidex.Presentation.iOS
{
	public partial class EventCardsController : BaseController
	{
		List<UserCard> FilterResults;
		List<UserCard> Cards;
		public EventTag SelectedTag { get; set; }

		public EventCardsController (IntPtr handle) : base (handle)
		{
		}

		void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			//Cards = Cards ?? new List<UserCard> ();

			if (Cards != null) {
				FilterResults.AddRange (
					Cards.Where (c => 
						(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant ().Contains (loweredFilter)) ||
						(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant ().Contains (loweredFilter)) ||
						(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant ().Contains (loweredFilter)) ||
						(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant ().Contains (loweredFilter)) ||
						(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter)))
					));

			}
			TableSource src = ConfigureTableSourceEventHandlers(FilterResults);
			src.IsFiltering = true;
			tblEventCards.Source = src;
			tblEventCards.ReloadData ();
			tblEventCards.AllowsSelection = true;
			tblEventCards.SetNeedsDisplay ();
		}

		void ResetFilter(){

			txtFilter.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Cards);
			src.NoCardsMessage = "No cards match your search";
			src.IsFiltering = false;
			tblEventCards.Source = src;
			tblEventCards.ReloadData ();
			tblEventCards.AllowsSelection = true;
			tblEventCards.SetNeedsDisplay ();
		}

		void ConfigureSearchBar(){

			txtFilter.Placeholder = "Filter";
			txtFilter.BarStyle = UIBarStyle.Default;
			txtFilter.ShowsCancelButton = true;

			txtFilter.SearchButtonClicked += delegate {
				SetFilter(txtFilter.Text);
				txtFilter.ResignFirstResponder();
			};
			txtFilter.CancelButtonClicked += delegate {
				ResetFilter();
				txtFilter.ResignFirstResponder();
			};
		}

		void GoToCard(){
			var cardController = Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)tblEventCards.Source).SelectedCard;

			if (cardController != null) {
				NavigationController.PushViewController (cardController, true);
			}
		}

		void ShowPhoneNumbers(){
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)tblEventCards.Source).SelectedCard;

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
				ShareCard (((TableSource)tblEventCards.Source).SelectedCard);
			};

			return src;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

