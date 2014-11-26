using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public delegate void ViewOrganizationHandler(long orgId);
	public delegate void ViewOrganizationMembersHandler(Organization org);
	public delegate void ViewOrganizationReferralsHandler(Organization org);

	public class OrganizationTableSource : BaseTableSource
	{

		List<Organization> Organizations;

		const float BASE_CELL_HEIGHT = 120f;

		public event ViewOrganizationHandler ViewOrganization;
		public event ViewOrganizationMembersHandler ViewOrganizationMembers;
		public event ViewOrganizationReferralsHandler ViewOrganizationReferrals;

		public OrganizationTableSource (List<Organization> organizations)
		{
			Organizations = new List<Organization> ();
			Organizations.AddRange (organizations);
			cellCache = new List<UITableViewCell> ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return Organizations.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return !Organizations.Any () ? BASE_CELL_HEIGHT * 3 : BASE_CELL_HEIGHT;
		}
			
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var organization = Organizations [indexPath.Row];

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

		void AddControls(UITableViewCell cell, Organization org){
		
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var fileName = Path.Combine (documentsPath, org.LogoFileName);

			if (!string.IsNullOrEmpty (org.LogoFileName)) {
				var frame = new RectangleF (10f, 10f, 220f, 80f);
				var imageFile = fileName + "." + org.LogoType;

				var orgImage = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.OrganizationImage) as UIImageView;
				if (orgImage != null) {
					orgImage.RemoveFromSuperview ();
				}
				orgImage = new UIImageView (frame);
				orgImage.Image = UIImage.FromFile (imageFile);
				orgImage.Tag = (int)UIElements.OrganizationImage;

				cell.ContentView.AddSubview (orgImage);


			} else {
				var NameLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.NameLabel) as UILabel;
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

			cell.Accessory = UITableViewCellAccessory.DetailButton;

//			var swiperShow = new UISwipeGestureRecognizer ();
//			swiperShow.Direction = UISwipeGestureRecognizerDirection.Left;
//			swiperShow.AddTarget (() => {
//				ClearOrgNavFromAllCells();
//				ShowPanel(cell);
//			});
//
//			var swiperHide = new UISwipeGestureRecognizer ();
//			swiperHide.Direction = UISwipeGestureRecognizerDirection.Right;
//			swiperHide.AddTarget (() => HidePanel (cell));
//
//			cell.ContentView.AddGestureRecognizer (swiperShow);
//			cell.ContentView.AddGestureRecognizer (swiperHide);

			AddSwipeView (ref cell, org);

		}

		UIButton getPanelButton(string title, CoreGraphics.CGRect frame, nfloat width, nfloat height){

			const float BORDER_RADIUS = 10f;
			const float BORDER_WIDTH = 1f;
		
			var button = new UIButton (frame);
			button.Layer.CornerRadius = BORDER_RADIUS;
			button.Layer.BorderWidth = BORDER_WIDTH;
			button.Layer.BackgroundColor = UIColor.White.CGColor;
			button.Layer.BorderColor =  UIColor.Blue.CGColor;
			button.SetTitle (title, UIControlState.Normal);
			button.SetTitleColor (UIColor.Blue, UIControlState.Normal);

			return button;
		}

		void AddSwipeView(ref UITableViewCell cell, Organization org){

			const float LEFT_MARGIN = 20f;
			const float TOP_MARGIN = 15f;
			const float BUTTON_WIDTH = 120f;
			const float BUTTON_HEIGHT = 45f;

			var frame = new RectangleF (320f, 0, 320, BASE_CELL_HEIGHT);
			var panel = new ButtonPanel (frame);

			var buttonFrame = new CoreGraphics.CGRect (10f, 10f, BUTTON_WIDTH, BUTTON_HEIGHT);

			var detailsButton = getPanelButton ("Details", buttonFrame, BUTTON_WIDTH, BUTTON_HEIGHT);
			detailsButton.TouchUpInside += delegate {
				ViewOrganization (org.OrganizationId);
			};
				
			buttonFrame.X += BUTTON_WIDTH + LEFT_MARGIN;

			var membersButton = getPanelButton ("Members", buttonFrame, BUTTON_WIDTH, BUTTON_HEIGHT);
			membersButton.TouchUpInside += delegate {
				ViewOrganizationMembers (org);
			};

			buttonFrame.X = 10f;
			buttonFrame.Y += BUTTON_HEIGHT + TOP_MARGIN;

			var referralsButton = getPanelButton ("Referrals", buttonFrame, BUTTON_WIDTH, BUTTON_HEIGHT);
			referralsButton.TouchUpInside += delegate {
				ViewOrganizationReferrals (org);
			};

			panel.AddSubview (detailsButton);
			panel.AddSubview (membersButton);
			panel.AddSubview (referralsButton);

			panel.BackgroundColor = UIColor.White;

			panel.Tag = (int)UIElements.ButtonPanel;

			cell.ContentView.AddSubview (panel);
		}

		// Analysis disable once UnusedParameter
		void ShowBrowser(string url){

//			if (this.ViewWebsite != null){
//				this.ViewWebsite (url);
//			}
		}
	}
}

