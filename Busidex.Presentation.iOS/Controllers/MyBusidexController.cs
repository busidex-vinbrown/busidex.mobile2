﻿using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using GoogleAnalytics.iOS;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	public partial class MyBusidexController : BaseCardViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		List<UserCard> FilterResults;
		const string NO_CARDS = "You Don't Have Any Cards In Your Collection. Search for some and add them!";
		bool loadingData;

		public MyBusidexController (IntPtr handle) : base (handle)
		{
		}

		void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			if (UISubscriptionService.UserCards != null) {
				FilterResults.AddRange (
					UISubscriptionService.UserCards.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant ().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant ().Contains (loweredFilter)) ||
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter))) ||
					(c.Card.Tags != null && c.Card.Tags.Any (t => t.Text.ToLowerInvariant ().Contains (loweredFilter)))
					));
			}
			TableSource src = ConfigureTableSourceEventHandlers(FilterResults);
			src.IsFiltering = true;
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		void ResetFilter(){

			if (SearchBar != null) {
				SearchBar.Text = string.Empty;
				TableSource src = ConfigureTableSourceEventHandlers (UISubscriptionService.UserCards);
				src.NoCardsMessage = NO_CARDS;
				src.IsFiltering = false;
				TableView.Source = src;
				TableView.ReloadData ();
				TableView.AllowsSelection = true;
				TableView.SetNeedsDisplay ();
			}
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNotes = true;
			src.ShowNoCardMessage = true;
			src.CardSelected += ShowCardActions;

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

		public void LoadMyBusidex(){

			if((UISubscriptionService.MyBusidexLoaded && CheckRefreshCookie (Resources.BUSIDEX_REFRESH_COOKIE_NAME))){
				ResetFilter();
				return;
			}

			var overlay = new MyBusidexLoadingOverlay (View.Bounds);
			overlay.MessageText = "Loading Your Cards";

			if (!UISubscriptionService.MyBusidexLoaded) {

				if(loadingData){
					return;
				}

				loadingData = true;

				View.AddSubview (overlay);

				OnMyBusidexUpdatedEventHandler update = status => InvokeOnMainThread (() => {
					overlay.TotalItems = status.Total;
					overlay.UpdateProgress (status.Count);
				});

				OnMyBusidexLoadedEventHandler callback = list => InvokeOnMainThread (() => {
					overlay.Hide ();
					ResetFilter ();
				});

				UISubscriptionService.OnMyBusidexUpdated += update;
				UISubscriptionService.OnMyBusidexLoaded += callback;

				UISubscriptionService.LoadUserCards ();
			}else{
				InvokeOnMainThread (() => {
					loadingData = false;
					overlay.Hide ();
					ResetFilter ();	
				});
			}
		}

		public void GoToTop(){
			if(TableView != null){
				TableView.SetContentOffset (new CGPoint (0, -75), true);
			}	
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			LoadMyBusidex ();
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

			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			ConfigureSearchBar ();

			var height = NavigationController.NavigationBar.Frame.Size.Height;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			SearchBar.Frame = new CGRect (0, height, UIScreen.MainScreen.Bounds.Width, 52);
		}
	}
}
