﻿using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.IO;
using System.Linq;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class OrgMembersController : BaseCardViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		public long OrganizationId{ get; set; }
		public string OrganizationName{ get; set; }
		public string OrganizationLogo{ get; set; }

		public enum MemberMode{
			Members = 1,
			Referrals = 2
		}
		List<UserCard> FilterResults;
		List<UserCard> Cards;

		const string NO_CARDS = "There are no {0} in this organization";

		public MemberMode OrganizationMemberMode{ get; set;}

		public OrgMembersController (IntPtr handle) : base (handle)
		{
		}

		void SetNavBarOrgImage(){
			var fileName = Path.Combine (documentsPath, OrganizationLogo);

			var imageFile = fileName;
			if (File.Exists (imageFile)) {
				var data = NSData.FromFile (imageFile);
				if (data != null) {
					imgLogo.Image = new UIImage (data);
				}
			}
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
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		void ResetFilter(){

			txtSearch.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Cards);
			src.NoCardsMessage = string.Format(NO_CARDS, (OrganizationMemberMode == MemberMode.Members ? "members" : "referrals"));
			src.IsFiltering = false;
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		void ConfigureSearchBar(){

			txtSearch.Placeholder = "Filter";
			txtSearch.BarStyle = UIBarStyle.Default;
			txtSearch.ShowsCancelButton = true;

			txtSearch.SearchButtonClicked += delegate {
				SetFilter(txtSearch.Text);
				txtSearch.ResignFirstResponder();
			};
			txtSearch.CancelButtonClicked += delegate {
				ResetFilter();
				txtSearch.ResignFirstResponder();
			};
			txtSearch.TextChanged += delegate {
				if(txtSearch.Text.Length == 0){
					ResetFilter();
					txtSearch.ResignFirstResponder();
				}
			};
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new OrgMemberTableSource (data);
			src.ShowNotes = false;
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No members have been loaded for this organization";
			src.CardSelected += ShowCardActions;

			return src;
		}

		void loadCards(){

			Cards = new List<UserCard> ();

			if (tblMembers.Source == null) {
				
				if(	OrganizationMemberMode == MemberMode.Members ){
					foreach(var card in UISubscriptionService.OrganizationMembers[OrganizationId] ){
						var userCard = new UserCard ();
						userCard.ExistsInMyBusidex = UISubscriptionService.UserCards.Exists(c => c.CardId == userCard.CardId);
						userCard.Card = card;
						userCard.CardId = card.CardId;

						Cards.Add (userCard);
					}
				}else{
					Cards.AddRange (UISubscriptionService.OrganizationReferrals[OrganizationId]);
				}

				ResetFilter ();
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			string name = OrganizationMemberMode == MemberMode.Members ? "Organization Members" : "Organization Referrals";

			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, name + " - " + OrganizationId);

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				SetNavBarOrgImage ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tblMembers.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			loadCards ();

			ConfigureSearchBar ();

			var height = NavigationController.NavigationBar.Frame.Size.Height;
			var width = UIScreen.MainScreen.Bounds.Width;
			height += UIApplication.SharedApplication.StatusBarFrame.Height;
			txtSearch.Frame = new CoreGraphics.CGRect (0, height, width, 52);

			height += txtSearch.Frame.Size.Height;
			imgLogo.Frame = new CoreGraphics.CGRect (0, height, width, 57);

			var top = height;
			height = UIScreen.MainScreen.Bounds.Height - top;

			tblMembers.Frame = new CoreGraphics.CGRect (0, top, width, height);
		}
	}
}
