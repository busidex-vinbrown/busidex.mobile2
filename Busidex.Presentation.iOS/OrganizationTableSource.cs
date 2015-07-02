using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public delegate void ViewOrganizationHandler(long orgId);
	public delegate void ViewOrganizationMembersHandler(Organization org);
	public delegate void ViewOrganizationReferralsHandler(Organization org);

	public class OrganizationTableSource : BaseTableSource
	{

		List<Organization> Organizations;

		public event ViewOrganizationHandler ViewOrganization;
		public event ViewOrganizationMembersHandler ViewOrganizationMembers;
		public event ViewOrganizationReferralsHandler ViewOrganizationReferrals;

		public OrganizationTableSource (List<Organization> organizations)
		{ 
			if(!organizations.Any()){
				ShowNoCardMessage = true;
				organizations.Add (new Organization ());
			}
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
			if(!Organizations.Any ()){
				return BASE_CELL_HEIGHT * 3;
			}
			if (Organizations [indexPath.Row].Visibility == 0) {
				return BASE_CELL_HEIGHT / 1.5f;
			}

			return BASE_CELL_HEIGHT / 3f;
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

			if (ShowNoCardMessage) {
				LoadNoCardMessage (cell);
			} else {
				// add controls here
				AddControls (cell, organization);
			}
			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		void AddControls(UITableViewCell cell, Organization org){
		
			var fileName = Path.Combine (documentsPath, org.LogoFileName);

			if (org.Visibility == 0) {
				if (!string.IsNullOrEmpty (org.LogoFileName)) {
					var frame = new CoreGraphics.CGRect (10f, 10f, UIScreen.MainScreen.Bounds.Width - 80f, 80f);
					var imageFile = fileName + "." + org.LogoType;

					var orgImage = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.OrganizationImage) as UIImageView;
					if (orgImage != null) {
						orgImage.RemoveFromSuperview ();
					}
					orgImage = new UIImageView (frame);
					orgImage.Image = UIImage.FromFile (imageFile);
					orgImage.Tag = (int)Resources.UIElements.OrganizationImage;

					cell.ContentView.AddSubview (orgImage);


				} else {
					var NameLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.NameLabel) as UILabel;
					if (NameLabel != null) {
						NameLabel.RemoveFromSuperview ();
					}

					var frame = new RectangleF (10f, 10f, 300f, 50f);
					NameLabel = new UILabel (frame);
					NameLabel.Tag = (int)Resources.UIElements.NameLabel;
					NameLabel.Text = org.Name;
					NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 18f);

					cell.ContentView.AddSubview (NameLabel);
				}
				cell.Accessory = UITableViewCellAccessory.DetailButton;
			}


			if (org.Visibility == 0) {
				AddSwipeView (ref cell, org);
			}else{
				AddLabelView (ref cell, org);
			}

		}

		static UIButton getPanelButton(string title, CoreGraphics.CGRect frame){

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

		void AddLabelView(ref UITableViewCell cell, Organization org){

			var NameLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.NameLabel) as UIButton;
			if (NameLabel != null) {
				NameLabel.RemoveFromSuperview ();
			}

			var frame = new RectangleF (10f, 10f, 300f, 50f);
			NameLabel = new UIButton (frame);
			NameLabel.Tag = (int)Resources.UIElements.NameLabel;
			NameLabel.SetTitle(org.Name, UIControlState.Normal);
			NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 18f);
			NameLabel.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			NameLabel.TouchUpInside += delegate {
				ViewOrganizationMembers (org);
			};

			cell.ContentView.AddSubview (NameLabel);
			cell.Accessory = UITableViewCellAccessory.None;

		}

		void AddSwipeView(ref UITableViewCell cell, Organization org){

			const float LEFT_MARGIN = 20f;
			const float TOP_MARGIN = 15f;
			const float BUTTON_WIDTH = 120f;
			const float BUTTON_HEIGHT = 45f;

			var panel = GetPanel ((float)UIScreen.MainScreen.Bounds.Width, BASE_CELL_HEIGHT / 1.5f);

			var buttonFrame = new CoreGraphics.CGRect (10f, 10f, BUTTON_WIDTH, BUTTON_HEIGHT);

			var detailsButton = getPanelButton ("Details", buttonFrame);
			detailsButton.TouchUpInside += delegate {
				ViewOrganization (org.OrganizationId);
			};
				
			buttonFrame.X += BUTTON_WIDTH + LEFT_MARGIN;

			var membersButton = getPanelButton ("Members", buttonFrame);
			membersButton.TouchUpInside += delegate {
				ViewOrganizationMembers (org);
			};

			buttonFrame.X = 10f;
			buttonFrame.Y += BUTTON_HEIGHT + TOP_MARGIN;

			var referralsButton = getPanelButton ("Referrals", buttonFrame);
			referralsButton.TouchUpInside += delegate {
				ViewOrganizationReferrals (org);
			};

			panel.AddSubview (detailsButton);
			panel.AddSubview (membersButton);
			panel.AddSubview (referralsButton);

			panel.BackgroundColor = UIColor.White;

			panel.Tag = (int)Resources.UIElements.ButtonPanel;

			cell.ContentView.AddSubview (panel);
		}
	}
}

