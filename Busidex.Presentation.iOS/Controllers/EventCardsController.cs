using System;
using UIKit;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using GoogleAnalytics.iOS;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class EventCardsController : BaseCardViewController
	{
		List<UserCard> FilterResults;
		List<UserCard> Cards;
		public EventTag SelectedTag { get; set; }
		const string NO_CARDS = "There are no cards in this event yet";

		public EventCardsController (IntPtr handle) : base (handle)
		{
		}

		void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			FilterResults.AddRange (
				Cards.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant ().Contains (loweredFilter)) ||
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter))) ||
					(c.Card.Tags != null && c.Card.Tags.Any(t => t.Text.ToLowerInvariant().Contains(loweredFilter)))
				));

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
			txtFilter.TextChanged += delegate {
				if(txtFilter.Text.Length == 0){
					ResetFilter();
					txtFilter.ResignFirstResponder();
				}
			};
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No cards match your search";
			src.CardSelected += ShowCardActions;

			return src;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			lblEventName.Text = SelectedTag.Description;

			var overlay = new MyBusidexLoadingOverlay (View.Bounds);
			overlay.MessageText = "Loading Event Cards";

			if (UISubscriptionService.EventCardsLoaded.ContainsKey(SelectedTag.Text) && UISubscriptionService.EventCardsLoaded[SelectedTag.Text]) {
				refreshTable (SelectedTag, UISubscriptionService.EventCards [SelectedTag.Text]);
				overlay.Hide ();
			} else {

				View.AddSubview (overlay);

				OnEventCardsLoadedEventHandler callback = (tag, list) => InvokeOnMainThread (() => {
					overlay.Hide ();
					refreshTable (tag, list);
				});

				UISubscriptionService.OnEventCardsLoaded += callback;

				UISubscriptionService.LoadEventCards (SelectedTag);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			if (SelectedTag != null) {
				GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Event - " + SelectedTag.Description);
			}
			base.ViewDidAppear (animated);

		}

		void refreshTable(EventTag tag, List<UserCard> cards){

			InvokeOnMainThread (() => {
				Cards = new List<UserCard> ();
				Cards.AddRange (cards);
				ResetFilter ();
				UISubscriptionService.OnEventCardsLoaded -= refreshTable;
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tblEventCards.RegisterClassForCellReuse (typeof(UITableViewCell), MyBusidexController.BusidexCellId);
			ConfigureSearchBar ();
		}
	}
}

