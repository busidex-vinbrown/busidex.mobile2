using System;
using UIKit;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using Foundation;
using System.Linq;
using System.Drawing;
using System.IO;

namespace Busidex.Presentation.iOS
{
	public delegate void ViewOrganizationHandler(long orgId);
	public delegate void ViewOrganizationMembersHandler(Organization org);
	public delegate void ViewOrganizationReferralsHandler(Organization org);

	public class OrganizationTableSource : UITableViewSource
	{

		private List<UITableViewCell> cellCache;
		private List<Organization> Organizations;

		private enum UIElements{
			OrganizationImage = 1,
			NameLabel = 2,
			WebsiteButton = 3,
			TwitterButton = 4,
			FacebookButton = 5,
			ButtonPanel = 6
		}

		private const float ANIMATION_SPEED = 0.5f;
		private const float BASE_CELL_HEIGHT = 120f;
		private const float LEFT_MARGIN = 5F;
		private const float LABEL_HEIGHT = 30f;
		private const float LABEL_WIDTH = 170f;
		private const float FEATURE_BUTTON_HEIGHT = 40f;
		private const float FEATURE_BUTTON_WIDTH = 40f;
		private const float FEATURE_BUTTON_MARGIN = 15f;
		public event ViewOrganizationHandler ViewOrganization;
		public event ViewOrganizationMembersHandler ViewOrganizationMembers;
		public event ViewOrganizationReferralsHandler ViewOrganizationReferrals;

		public OrganizationTableSource (List<Organization> organizations)
		{
			this.Organizations = new List<Organization> ();
			this.Organizations.AddRange (organizations);
			cellCache = new List<UITableViewCell> ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return Organizations.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return Organizations.Count() == 0 ? BASE_CELL_HEIGHT * 3 : BASE_CELL_HEIGHT;
		}
			
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var organization = (Organization)Organizations [indexPath.Row];

			var cell = tableView.DequeueReusableCell (OrganizationsController.BusidexCellId, indexPath);

			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
			if (cellCache.All (c => c.Tag != indexPath.Row)) {
				cellCache.Add (cell);
			} 

			// add controls here
			AddControls (cell, organization);

			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}

		private void AddControls(UITableViewCell cell, Organization org){
		
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var fileName = System.IO.Path.Combine (documentsPath, org.LogoFileName);

			if (!string.IsNullOrEmpty (org.LogoFileName)) {
				var frame = new RectangleF (10f, 10f, 220f, 80f);
				var imageFile = fileName + "." + org.LogoType;

				var orgImage = cell.ContentView.Subviews.Where (s => s.Tag == (int)UIElements.OrganizationImage).SingleOrDefault () as UIImageView;
				if (orgImage != null) {
					orgImage.RemoveFromSuperview ();
				}
				orgImage = new UIImageView (frame);
				orgImage.Image = UIImage.FromFile (imageFile);
				orgImage.Tag = (int)UIElements.OrganizationImage;

				cell.ContentView.AddSubview (orgImage);


			} else {
				var NameLabel = cell.ContentView.Subviews.Where (s => s.Tag == (int)UIElements.NameLabel).SingleOrDefault () as UILabel;
				if (NameLabel != null) {
					NameLabel.RemoveFromSuperview ();
				}

				var frame = new RectangleF (10f, 10f, 300f, 50f);
				NameLabel = new UILabel (frame);
				NameLabel.Tag = (int)UIElements.NameLabel;
				NameLabel.Text = org.Name;
				NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 18f);

				cell.ContentView.AddSubview (NameLabel);
			}

			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			UISwipeGestureRecognizer swiperShow = new UISwipeGestureRecognizer ();
			swiperShow.Direction = UISwipeGestureRecognizerDirection.Left;
			swiperShow.AddTarget (() => {
				ClearOrgNavFromAllCells();
				ShowOrgNav(cell);
			});

			UISwipeGestureRecognizer swiperHide = new UISwipeGestureRecognizer ();
			swiperHide.Direction = UISwipeGestureRecognizerDirection.Right;
			swiperHide.AddTarget (() => {
				HideOrgNav(cell);
			});

			cell.ContentView.AddGestureRecognizer (swiperShow);
			cell.ContentView.AddGestureRecognizer (swiperHide);

			AddSwipeView (ref cell, org);

		}

		public void ClearOrgNavFromAllCells(){

			foreach(UITableViewCell cell in cellCache){
				HideOrgNav (cell);
			}
		}

		private void ShowOrgNav(UITableViewCell cell){
			var ButtonPanel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.ButtonPanel) as UIView;
			UIView.Animate (ANIMATION_SPEED, () => {
				ButtonPanel.Frame = new CoreGraphics.CGRect(0, ButtonPanel.Frame.Location.Y, ButtonPanel.Frame.Size.Width, ButtonPanel.Frame.Size.Height);
			});
		}

		private void HideOrgNav(UITableViewCell cell){
			var ButtonPanel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.ButtonPanel) as UIView;
			UIView.Animate (ANIMATION_SPEED, () => {
				ButtonPanel.Frame = new CoreGraphics.CGRect(ButtonPanel.Frame.Width, ButtonPanel.Frame.Location.Y, ButtonPanel.Frame.Size.Width, ButtonPanel.Frame.Size.Height);
			});
		}

		private void AddSwipeView(ref UITableViewCell cell, Organization org){

			var frame = new RectangleF (320f, 0, 320, BASE_CELL_HEIGHT);
			var panel = new UIView (frame);
			var buttonWidth = 130f;
			var buttonHeight = 45f;
			var leftMargin = 20f;
			var topMargin = 15f;

			var buttonFrame = new RectangleF (10f, 10f, buttonWidth, buttonHeight);

			var detailsButton = new UIButton (buttonFrame);
			detailsButton.BackgroundColor = UIColor.Blue;
			detailsButton.SetTitle ("Details", UIControlState.Normal);
			detailsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			detailsButton.TouchUpInside += delegate {
				ViewOrganization (org.OrganizationId);
			};
				
			buttonFrame.X += buttonWidth + leftMargin;

			var membersButton = new UIButton (buttonFrame);
			membersButton.BackgroundColor = UIColor.Blue;
			membersButton.SetTitle ("Members", UIControlState.Normal);
			membersButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			membersButton.TouchUpInside += delegate {
				ViewOrganizationMembers (org);
			};

			buttonFrame.X = 10f;
			buttonFrame.Y += buttonHeight + topMargin;

			var referralsButton = new UIButton (buttonFrame);
			referralsButton.BackgroundColor = UIColor.Blue;
			referralsButton.SetTitle ("Referrals", UIControlState.Normal);
			referralsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			referralsButton.TouchUpInside += delegate {
				ViewOrganizationReferrals (org);
			};

			buttonFrame.X += buttonWidth + leftMargin;
			var sendReferralsButton = new UIButton (buttonFrame);
			sendReferralsButton.BackgroundColor = UIColor.Blue;
			sendReferralsButton.SetTitle ("Send Referrals", UIControlState.Normal);
			sendReferralsButton.SetTitleColor (UIColor.White, UIControlState.Normal);

			panel.AddSubview (detailsButton);
			panel.AddSubview (membersButton);
			panel.AddSubview (referralsButton);
			panel.AddSubview (sendReferralsButton);

			panel.BackgroundColor = UIColor.White;

			panel.Tag = (int)UIElements.ButtonPanel;

			cell.ContentView.AddSubview (panel);
		}

		private void AddFeatureButtons(UITableViewCell cell, List<UIButton> FeatureButtons){
		
			float buttonY = 40f + LABEL_HEIGHT;
			float buttonX =  LEFT_MARGIN;

			var cellButtons = cell.ContentView.Subviews.Where (s => s is UIButton).ToList ();
			foreach(var button in cellButtons){
				button.RemoveFromSuperview ();
			}

			var frame = new RectangleF (buttonX, buttonY, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
			float buttonXOriginal = buttonX;
			int idx = 0;
			foreach(var button in FeatureButtons.OrderBy(b=>b.Tag)){

				button.Frame = frame;
				cell.ContentView.AddSubview (button);

				idx++;
				if (idx % 3 == 0) { 
					buttonX = buttonXOriginal;
					frame.Y += FEATURE_BUTTON_HEIGHT + 10f;
				} else {
					buttonX += FEATURE_BUTTON_WIDTH + FEATURE_BUTTON_MARGIN;
				}
				frame.X = buttonX;
			}
		}

		private void ShowBrowser(string url){

//			if (this.ViewWebsite != null){
//				this.ViewWebsite (url);
//			}
		}
	}
}

