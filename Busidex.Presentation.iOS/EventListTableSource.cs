using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using MyBusidexController = Busidex.Presentation.iOS.Controllers.MyBusidexController;

namespace Busidex.Presentation.iOS
{
	public delegate void ItemSelectedHandler(EventTag tag);

	public class EventListTableSource : UITableViewSource
	{

		public event ItemSelectedHandler OnItemSelected;

		const float BASE_CELL_HEIGHT = 50f;
		const float TOP_MARGIN = 10f;
		List<UITableViewCell> cellCache;
		List<EventTag> EventList;

		public EventListTableSource (List<EventTag> items)
		{
			EventList = new List<EventTag>();

			if(items != null){
				EventList.AddRange (items);
			}
			cellCache = new List<UITableViewCell> ();
		}

		protected void SelectItem(EventTag tag){
			if (OnItemSelected != null) {
				OnItemSelected (tag);
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return EventList.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return BASE_CELL_HEIGHT;
		}

		protected void AddTagLabel(EventTag tag, UITableViewCell cell){
			var TagLabel = cell.ContentView.Subviews.SingleOrDefault(s=> s is UIButton) as UIButton;
			var frame = new CoreGraphics.CGRect (5f, TOP_MARGIN, UIScreen.MainScreen.Bounds.Width, BASE_CELL_HEIGHT - TOP_MARGIN);
			if (TagLabel == null) {
				TagLabel = new UIButton (frame);
			}else{
				TagLabel.Frame = frame;
			}
			TagLabel.Tag = (int)Resources.UIElements.NameLabel;
			TagLabel.SetTitle (tag.Description, UIControlState.Normal);
			TagLabel.Font = UIFont.FromName ("Helvetica-Bold", 16f);
			TagLabel.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			TagLabel.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			TagLabel.TouchUpInside += delegate {
				SelectItem(tag);
			};
			cell.ContentView.AddSubview (TagLabel);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (MyBusidexController.BusidexCellId, indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
			AddTagLabel (EventList [indexPath.Row], cell);

			if (cellCache.All (c => c.Tag != indexPath.Row)) {
				cellCache.Add (cell);
			}

			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}
	}
}

