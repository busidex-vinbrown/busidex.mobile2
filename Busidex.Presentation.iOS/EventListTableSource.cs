using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public delegate void ItemSelected();

	public class EventListTableSource : UITableViewSource
	{

		const float BASE_CELL_HEIGHT = 180f;
		List<UITableViewCell> cellCache;
		List<EventTag> EventList;
		public EventTag SelectedEvent;

		public EventListTableSource (List<EventTag> items)
		{
			EventList = new List<EventTag>();

			if(items != null){
				EventList.AddRange (items);
			}
			cellCache = new List<UITableViewCell> ();
		}

		public event ItemSelected OnItemSelected;

		protected void SelectItem(int idx){
			SelectedEvent = EventList [idx];
			if (OnItemSelected != null) {
				OnItemSelected ();
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

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (MyBusidexController.BusidexCellId, indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
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

