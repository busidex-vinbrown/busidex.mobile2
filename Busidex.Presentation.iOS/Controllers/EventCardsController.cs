﻿using System;
using System.Collections.Generic;
using System.Linq;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class EventCardsController : BaseCardViewController
	{
		List<UserCard> FilterResults;
		List<UserCard> Cards;

		public EventTag SelectedTag { get; set; }

		const string NO_CARDS = "There are no cards in this event yet";
		Dictionary<string, MyBusidexLoadingOverlay> overlays;

		public EventCardsController (IntPtr handle) : base (handle)
		{
		}

		void SetFilter (string filter)
		{
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			FilterResults.AddRange (
				Cards.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant ().Contains (loweredFilter)) ||
				(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant ().Contains (loweredFilter)) ||
				(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant ().Contains (loweredFilter)) ||
				(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant ().Contains (loweredFilter)) ||
				(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter))) ||
				(c.Card.Tags != null && c.Card.Tags.Any (t => t.Text.ToLowerInvariant ().Contains (loweredFilter)))
				));

			TableSource src = ConfigureTableSourceEventHandlers (FilterResults);
			src.IsFiltering = true;
			tblEventCards.Source = src;
			tblEventCards.ReloadData ();
			tblEventCards.AllowsSelection = true;
			tblEventCards.SetNeedsDisplay ();
		}

		void ResetFilter ()
		{

			txtFilter.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers (Cards);
			src.NoCardsMessage = "No cards match your search";
			src.IsFiltering = false;
			tblEventCards.Source = src;
			tblEventCards.ReloadData ();
			tblEventCards.AllowsSelection = true;
			tblEventCards.SetNeedsDisplay ();
		}

		void ConfigureSearchBar ()
		{

			txtFilter.Placeholder = "Filter";
			txtFilter.BarStyle = UIBarStyle.Default;
			txtFilter.ShowsCancelButton = true;

			txtFilter.SearchButtonClicked += delegate {
				SetFilter (txtFilter.Text);
				txtFilter.ResignFirstResponder ();
			};
			txtFilter.CancelButtonClicked += delegate {
				ResetFilter ();
				txtFilter.ResignFirstResponder ();
			};
			txtFilter.TextChanged += delegate {
				if (txtFilter.Text.Length == 0) {
					ResetFilter ();
					txtFilter.ResignFirstResponder ();
				}
			};
		}

		TableSource ConfigureTableSourceEventHandlers (List<UserCard> data)
		{
			var src = new TableSource (data);
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No cards match your search";
			src.CardSelected += ShowCardActions;

			return src;
		}

		void resetList ()
		{
			Cards = new List<UserCard> ();
			Cards.AddRange (UISubscriptionService.EventCards [SelectedTag.Text]);
			Cards.ForEach (card => {
				var mbCard = UISubscriptionService.UserCards.FirstOrDefault (c => c.CardId == card.CardId);
				if (mbCard != null) {
					card.Notes = mbCard.Notes;	
				}
			});
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			lblEventName.Text = SelectedTag.Description;

			if (UISubscriptionService.EventCardsLoaded.ContainsKey (SelectedTag.Text) && UISubscriptionService.EventCardsLoaded [SelectedTag.Text]) {
				resetList ();
				ResetFilter ();
				overlays [SelectedTag.Text].Hide ();
			} else {

				if (!overlays.ContainsKey (SelectedTag.Text)) {
					overlays.Add (SelectedTag.Text, new MyBusidexLoadingOverlay (View.Bounds));
					overlays [SelectedTag.Text].MessageText = "Loading Event Cards";
					overlays [SelectedTag.Text].Hidden = false;
					View.AddSubview (overlays [SelectedTag.Text]);
				}else{
					overlays [SelectedTag.Text].Hidden = false;
				}

				OnEventCardsLoadedEventHandler callback = (tag, list) => {

					resetList ();

					InvokeOnMainThread (() => {
						overlays [SelectedTag.Text].Hide ();
						ResetFilter ();
					});
				};

				UISubscriptionService.EventCardsLoadedEventTable [SelectedTag.Text] += callback;

				UISubscriptionService.LoadEventCards (SelectedTag);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			if (SelectedTag != null) {
				Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Event - " + SelectedTag.Description);
			}
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			overlays = new Dictionary<string, MyBusidexLoadingOverlay> ();

			OnEventCardsUpdatedEventHandler update = status => InvokeOnMainThread (() => {
				if (IsViewLoaded && View.Window != null) {  // no need to show anything if the view isn't visible any more
					overlays [SelectedTag.Text].TotalItems = status.Total;
					overlays [SelectedTag.Text].UpdateProgress (status.Count);
				}
			});

			UISubscriptionService.OnEventCardsUpdated += update;

			tblEventCards.RegisterClassForCellReuse (typeof(UITableViewCell), MyBusidexController.BusidexCellId);
			ConfigureSearchBar ();
		}
	}
}

