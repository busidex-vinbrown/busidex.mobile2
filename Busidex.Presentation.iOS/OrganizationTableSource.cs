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
	public delegate void ViewOrganizationCardsHandler(Organization org, OrgMembersController.MemberMode mode);

	public class OrganizationTableSource : BaseTableSource
	{

		List<Organization> Organizations;

		public event ViewOrganizationHandler ViewOrganization;
		public event ViewOrganizationCardsHandler ViewOrganizationCards;

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
			if (Organizations [indexPath.Row].IsMember) {
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

			if (org.IsMember) {
				if (!string.IsNullOrEmpty (org.LogoFileName)) {
					var frame = new CoreGraphics.CGRect (10f, 10f, UIScreen.MainScreen.Bounds.Width - 80f, 80f);
					var imageFile = fileName + "." + org.LogoType;

					var btnOrgImage = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.OrganizationImage) as UIButton;
					if (btnOrgImage != null) {
						btnOrgImage.RemoveFromSuperview ();
					}
					btnOrgImage = new UIButton (frame);
					btnOrgImage.SetImage(UIImage.FromFile (imageFile), UIControlState.Normal);
					btnOrgImage.Tag = (int)Resources.UIElements.OrganizationImage;

					btnOrgImage.TouchUpInside += delegate {
						ViewOrganization(org.OrganizationId);
					};

					cell.ContentView.AddSubview (btnOrgImage);
				} else {
					AddLabelView (ref cell, org);
				}
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
				ViewOrganizationCards (org, OrgMembersController.MemberMode.Members);
			};

			cell.ContentView.AddSubview (NameLabel);
		}
	}
}

