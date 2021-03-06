﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class SearchController : BaseCardViewController
	{
		public static NSString cellID = new NSString ("cellId");

		public SearchController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Search");

			base.ViewDidAppear (animated);
		}

		public static bool SearchButtonHandlerAssigned;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			//ResetUI ();
		}

		void ResetUI ()
		{
			txtSearch.Text = string.Empty;

			var src = ConfigureTableSourceEventHandlers (new List<UserCard> ());
			vwSearchResults.Source = src;
			vwSearchResults.ReloadData ();
			vwSearchResults.Hidden = true;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwSearchResults.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);

			SearchButtonHandlerAssigned = true;

			txtSearch.TextChanged += delegate {
				vwSearchResults.Hidden |= txtSearch.Text.Length == 0;
			};

			txtSearch.CancelButtonClicked += delegate {
				txtSearch.Text = string.Empty;
				vwSearchResults.Hidden = true;
			};

			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
				txtSearch.ResignFirstResponder ();
					
				try {
					DoSearch ();
				} catch (AggregateException ex) {
					Xamarin.Insights.Report (ex);
				}

			};
			txtSearch.CancelButtonClicked += delegate {
				txtSearch.ResignFirstResponder ();
			};
			var height = NavigationController.NavigationBar.Frame.Size.Height;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			txtSearch.Frame = new CoreGraphics.CGRect (0, height, UIScreen.MainScreen.Bounds.Width, 52);
			ResetUI ();
		}

		void ShowPhoneNumbers ()
		{
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.SelectedCard = ((TableSource)vwSearchResults.Source).SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}
		}

		TableSource ConfigureTableSourceEventHandlers (List<UserCard> data)
		{
			var src = new TableSource (data);
			src.ShowNotes = false;
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No cards match your search";
			src.CardSelected += ShowCardActions;

			return src;
		}

		void LoadSearchResults (List<UserCard> cards)
		{

			var src = ConfigureTableSourceEventHandlers (cards); 
			vwSearchResults.Hidden = false;
			vwSearchResults.Source = src;
			vwSearchResults.ReloadData ();
			vwSearchResults.AllowsSelection = true;
			vwSearchResults.SetNeedsDisplay ();

			Overlay.Hide ();
		}

		protected override void StartSearch ()
		{

			base.StartSearch ();

			var src = new TableSource (new List<UserCard> ());

			vwSearchResults.Source = src;
			vwSearchResults.ReloadData ();
			vwSearchResults.AllowsSelection = true;
			vwSearchResults.SetNeedsDisplay ();

			View.SetNeedsDisplay ();
		}

		protected void DoSearch ()
		{

			var ctrl = new Mobile.SearchController ();
			ctrl.DoSearch (txtSearch.Text, UISubscriptionService.AuthToken).ContinueWith (response => {

				var cards = new List<UserCard> ();

				if (response == null || response.Result == null || string.IsNullOrEmpty (response.Result)) {
					InvokeOnMainThread (() => LoadSearchResults (cards));
					return;
				}

				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response.Result);

				if (Search.SearchModel == null || Search.SearchModel.Results == null || !Search.SearchModel.Results.Any ()) {
					InvokeOnMainThread (() => LoadSearchResults (new List<UserCard> ()));
				} else {

					float total = Search.SearchModel.Results.Count;
					float processed = 0;

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
									if (processed.Equals (total)) {
										InvokeOnMainThread (() => LoadSearchResults (cards));
									} 
								});
							} else {
								processed++;
								if (processed.Equals (total)) {
									InvokeOnMainThread (() => LoadSearchResults (cards));
								}
							}
						}
					}
				}
			});
		}
	}
}
