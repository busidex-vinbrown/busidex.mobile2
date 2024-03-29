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
	public partial class OrgMembersController : BaseCardViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public long OrganizationId { get; set; }

		public string OrganizationName { get; set; }

		public string OrganizationLogo { get; set; }

		public bool IsOrgMember { get; set; }

		public enum MemberMode
		{
			Members = 1,
			Referrals = 2
		}

		List<UserCard> FilterResults;
		List<UserCard> Cards;

		const string NO_CARDS = "There are no {0} in this organization";

		public MemberMode OrganizationMemberMode { get; set; }

		MyBusidexLoadingOverlay overlay;

		public OrgMembersController (IntPtr handle) : base (handle)
		{
		}

		void SetNavBarOrgImage ()
		{
			var fileName = Path.Combine (documentsPath, OrganizationLogo);

			var imageFile = fileName;
			if (File.Exists (imageFile)) {
				var data = NSData.FromFile (imageFile);
				if (data != null) {
					btnOrgImage.SetBackgroundImage (new UIImage (data), UIControlState.Normal);
				}
			}
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
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		void ResetFilter ()
		{

			txtSearch.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers (Cards);
			src.NoCardsMessage = string.Format (NO_CARDS, (OrganizationMemberMode == MemberMode.Members ? "members" : "referrals"));
			src.IsFiltering = false;
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		void ConfigureSearchBar ()
		{

			txtSearch.Placeholder = "Filter";
			txtSearch.BarStyle = UIBarStyle.Default;
			txtSearch.ShowsCancelButton = true;

			txtSearch.SearchButtonClicked += delegate {
				SetFilter (txtSearch.Text);
				txtSearch.ResignFirstResponder ();
			};
			txtSearch.CancelButtonClicked += delegate {
				ResetFilter ();
				txtSearch.ResignFirstResponder ();
			};
			txtSearch.TextChanged += delegate {
				if (txtSearch.Text.Length == 0) {
					ResetFilter ();
					txtSearch.ResignFirstResponder ();
				}
			};
		}

		TableSource ConfigureTableSourceEventHandlers (List<UserCard> data)
		{
			var src = new OrgMemberTableSource (data);
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No members have been loaded for this organization";
			src.CardSelected += ShowCardActions;

			return src;
		}

		async void loadCards ()
		{

			Cards = new List<UserCard> ();

			if (OrganizationMemberMode == MemberMode.Members) {
				if (!UISubscriptionService.OrganizationMembers.ContainsKey (OrganizationId) ||
					UISubscriptionService.OrganizationMembers [OrganizationId] == null ||
					!UISubscriptionService.OrganizationMembersLoaded.ContainsKey (OrganizationId) ||
					!UISubscriptionService.OrganizationMembersLoaded [OrganizationId]) {

					InvokeOnMainThread (() => {
						overlay = new MyBusidexLoadingOverlay (View.Bounds);
						overlay.MessageText = "Loading " + OrganizationName;
						View.AddSubview (overlay);
					});

					OnMyOrganizationMembersUpdatedEventHandler update = status => InvokeOnMainThread (() => {
						if (IsViewLoaded && View.Window != null) {  // no need to show anything if the view isn't visible any more
							overlay.TotalItems = status.Total;
							InvokeOnMainThread (() => {
								overlay.UpdateProgress (status.Count);
							});
						}
					});

					OnMyOrganizationMembersLoadedEventHandler callback = list => InvokeOnMainThread (populateMembers);

					UISubscriptionService.OnMyOrganizationMembersUpdated -= update;
					UISubscriptionService.OnMyOrganizationMembersUpdated += update;
					UISubscriptionService.OrganizationMembersLoadedEventTable [OrganizationId] += callback;

					await UISubscriptionService.LoadOrganizationMembers (OrganizationId);

				} else {
					populateMembers ();
				}
			} else {
				if (!UISubscriptionService.OrganizationReferrals.ContainsKey (OrganizationId) ||
					UISubscriptionService.OrganizationReferrals [OrganizationId] == null ||
					!UISubscriptionService.OrganizationReferralsLoaded.ContainsKey (OrganizationId) ||
					!UISubscriptionService.OrganizationReferralsLoaded [OrganizationId]) {

					InvokeOnMainThread (() => {
						overlay = new MyBusidexLoadingOverlay (View.Bounds);
						overlay.MessageText = "Loading " + OrganizationName;
						View.AddSubview (overlay);
					});

					OnMyOrganizationReferralsUpdatedEventHandler update = status => InvokeOnMainThread (() => {
						if (IsViewLoaded && View.Window != null) {  // no need to show anything if the view isn't visible any more
							overlay.TotalItems = status.Total;
							overlay.UpdateProgress (status.Count);
						}
					});

					OnMyOrganizationReferralsLoadedEventHandler callback = list => InvokeOnMainThread (populateReferrals);

					UISubscriptionService.OnMyOrganizationReferralsUpdated -= update;
					UISubscriptionService.OnMyOrganizationReferralsUpdated += update;

					UISubscriptionService.OrganizationReferralsLoadedEventTable [OrganizationId] += callback;

					await UISubscriptionService.LoadOrganizationReferrals (OrganizationId);
				} else {
					populateReferrals ();
				}
			}
		}

		void populateMembers ()
		{
			if (UISubscriptionService.OrganizationMembers.ContainsKey (OrganizationId)) {
				foreach (var card in UISubscriptionService.OrganizationMembers [OrganizationId]) {
					var userCard = new UserCard ();

					var mbCard = UISubscriptionService.UserCards.FirstOrDefault (c => c.CardId == card.CardId);

					userCard.ExistsInMyBusidex = mbCard != null;
					userCard.Card = card;
					userCard.CardId = card.CardId;
					if (userCard.ExistsInMyBusidex) {
						userCard.Notes = mbCard.Notes;
					}

					Cards.Add (userCard);
				}
			}
			if (overlay != null) {
				overlay.Hide ();
			}
			InvokeOnMainThread (ResetFilter);
		}

		void populateReferrals ()
		{
			if (UISubscriptionService.OrganizationReferrals.ContainsKey (OrganizationId)) {
				Cards.AddRange (UISubscriptionService.OrganizationReferrals [OrganizationId]);

				Cards.ForEach (card => {
					var mbCard = UISubscriptionService.UserCards.FirstOrDefault (c => c.CardId == card.CardId);
					if (mbCard != null) {
						card.Notes = mbCard.Notes;
					}
				});
			}
			if (overlay != null) {
				overlay.Hide ();
			}
			InvokeOnMainThread (ResetFilter);
		}

		public override void ViewDidAppear (bool animated)
		{
			string name = OrganizationMemberMode == MemberMode.Members ? "Organization Members" : "Organization Referrals";

			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, name + " - " + OrganizationId);

			base.ViewDidAppear (animated);

			btnOrgImage.TouchUpInside += delegate {
				if (IsOrgMember) {
					if (NavigationController.ViewControllers.Any (c => c as OrganizationDetailController != null)) {
						NavigationController.PopToViewController (BaseNavigationController.orgDetailController, true);
					} else {
						NavigationController.PushViewController (BaseNavigationController.orgDetailController, true);
					}
				}
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				SetNavBarOrgImage ();

				loadCards ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tblMembers.RegisterClassForCellReuse (typeof (UITableViewCell), BusidexCellId);

			ConfigureSearchBar ();

			var height = NavigationController.NavigationBar.Frame.Size.Height;
			var width = UIScreen.MainScreen.Bounds.Width;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			txtSearch.Frame = new CoreGraphics.CGRect (0, height, width, 52);

			height += txtSearch.Frame.Size.Height;
			btnOrgImage.Frame = new CoreGraphics.CGRect (0, height, width, 57);


			var top = height;
			height = UIScreen.MainScreen.Bounds.Height - top;

			tblMembers.Frame = new CoreGraphics.CGRect (0, top, width, height);


		}
	}
}
