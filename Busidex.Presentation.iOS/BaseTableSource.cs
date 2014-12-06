using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class BaseTableSource : UITableViewSource
	{
		protected const float BASE_CELL_HEIGHT = 180f;

		const float LEFT_MARGIN = 5F;
		const float LABEL_HEIGHT = 30f;
		const float LABEL_WIDTH = 170f;
		const float FEATURE_BUTTON_HEIGHT = 40f;
		const float FEATURE_BUTTON_WIDTH = 40f;
		const float FEATURE_BUTTON_MARGIN = 15f;
		protected const string NONE_MATCH_FILTER = "No cards match your filter";

		UIColor CELL_BACKGROUND_COLOR = UIColor.FromRGB (240, 239, 243);

		protected List<UITableViewCell> cellCache;
		public string NoCardsMessage{ get; set;}
		public bool ShowNoCardMessage{ get; set; }
		public bool IsFiltering{ get; set;}

		protected string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		protected enum UIElements{
			CardImage = 1,
			NameLabel = 2,
			CompanyLabel = 3,
			MapButton = 4,
			NotesButton = 5,
			EmailButton = 6,
			WebsiteButton = 7,
			PhoneNumberButton = 8,
			AddToMyBusidexButton = 9,
			RemoveFromMyBusidexButton = 10,
			OrganizationImage = 11,
			TwitterButton = 12,
			FacebookButton = 13,
			ButtonPanel = 14

		}

		protected void LoadNoCardMessage(UITableViewCell cell){

			const float LABEL_HEIGHT = 61f * 3;
			const float LABEL_WIDTH = 280f;

			var frame = new RectangleF (10f, 10f, LABEL_WIDTH, LABEL_HEIGHT);

			var lbl = new UILabel (frame);
			lbl.Text = IsFiltering ? NONE_MATCH_FILTER : NoCardsMessage;
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Font = UIFont.FromName ("Helvetica", 17f);
			lbl.Lines = 3;

			foreach(var view in cell.ContentView.Subviews){
				view.RemoveFromSuperview ();
			}
			lbl.Tag = -1;
			cell.ContentView.AddSubview (lbl);

			cell.Frame = frame;

		}

		protected ButtonPanel GetPanel(float width, float height){
			var frame = new CoreGraphics.CGRect (width, 0, width, height);
			var panel = new ButtonPanel (frame);
			panel.Tag = (int)UIElements.ButtonPanel;
			panel.BackgroundColor = UIColor.White;

			return panel;
		}

		protected bool PanelVisible{ get; set;}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 1;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return BASE_CELL_HEIGHT;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return tableView.CellAt (indexPath);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			tableView.BackgroundColor =	cell.ContentView.BackgroundColor = cell.BackgroundColor = CELL_BACKGROUND_COLOR;
		}

		protected void AddFeatureButtons(UITableViewCell cell, List<UIButton> FeatureButtons){

			var panel = cell.ContentView.Subviews.SingleOrDefault(v=> v.Tag == (int)UIElements.ButtonPanel) ?? GetPanel ((float)UIScreen.MainScreen.Bounds.Width, BASE_CELL_HEIGHT);

			const float FEATURE_BUTTON_TOP_MARGIN = 10f;

			float buttonX = LEFT_MARGIN;

			var buttons = panel.Subviews.ToList ();
			foreach(var button in buttons){
				button.RemoveFromSuperview ();
			}

			var frame = new RectangleF (buttonX, FEATURE_BUTTON_TOP_MARGIN, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
			float buttonXOriginal = buttonX;
			int idx = 0;
			var list = FeatureButtons.OrderBy (b => (int)b.Tag).ToList ();
			foreach(var button in list){

				frame.X = buttonX;
				buttonX += FEATURE_BUTTON_WIDTH + FEATURE_BUTTON_MARGIN;

				button.Frame = frame;
				panel.AddSubview (button);

				idx++;
				if (idx % 4 == 0) { 
					buttonX = buttonXOriginal;
					frame.Y += FEATURE_BUTTON_HEIGHT + 10f;
				}
			}
			cell.ContentView.AddSubview (panel);
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath);

			ClearOrgNavFromAllCells (cell);

			var panel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.ButtonPanel) as ButtonPanel;

			if(panel != null){
				panel.Toggle();
			}
		}

		protected void ClearOrgNavFromAllCells(UITableViewCell currentCell){

			if (cellCache != null) {
				foreach (UITableViewCell cell in cellCache) {
					if (cell != currentCell) {
						var panel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.ButtonPanel) as ButtonPanel;
						if (panel != null)
							panel.Hide ();
					}
				}
			}
		}
	}
}

