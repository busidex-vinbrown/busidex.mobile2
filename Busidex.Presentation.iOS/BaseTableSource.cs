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
	public class BaseTableSource : UITableViewSource
	{
		const float BASE_CELL_HEIGHT = 120f;

		const float LEFT_MARGIN = 5F;
		const float LABEL_HEIGHT = 30f;
		const float LABEL_WIDTH = 170f;
		const float FEATURE_BUTTON_HEIGHT = 40f;
		const float FEATURE_BUTTON_WIDTH = 40f;
		const float FEATURE_BUTTON_MARGIN = 15f;

		protected List<UITableViewCell> cellCache;

		protected enum UIElements{
			OrganizationImage = 1,
			NameLabel = 2,
			WebsiteButton = 3,
			TwitterButton = 4,
			FacebookButton = 5,
			ButtonPanel = 6
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

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}

		protected static void AddFeatureButtons(UITableViewCell cell, IEnumerable<UIButton> featureButtons){

			const float BUTTON_Y = 40f + LABEL_HEIGHT;
			float buttonX =  LEFT_MARGIN;

			var cellButtons = cell.ContentView.Subviews.Where (s => s is UIButton).ToList ();
			foreach(var button in cellButtons){
				button.RemoveFromSuperview ();
			}

			var frame = new RectangleF (buttonX, LEFT_MARGIN, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
			float buttonXOriginal = buttonX;
			int idx = 0;
			foreach(var button in featureButtons.OrderBy(b=>b.Tag)){

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

