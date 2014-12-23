using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public class SharedCardTableSource : BaseTableSource
	{
		public delegate void SaveSharedCardHandler(List<SharedCard> sharedCards);

		List<SharedCard> SharedCards { get; set; }

		public SharedCardTableSource (List<SharedCard> sharedCards)
		{
			SharedCards = sharedCards;
			Cards = new List<UserCard> ();

			Cards.AddRange (sharedCards.Select(c => new UserCard{Card = c.Card, CardId = c.Card.CardId}));

			cellCache = new List<UITableViewCell> ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return SharedCards.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return !SharedCards.Any () ? BASE_CELL_HEIGHT * 3 : BASE_CELL_HEIGHT;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var sharedCard = SharedCards [indexPath.Row];

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
				AddControls (cell, sharedCard);
			}
			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		void AddControls(UITableViewCell cell, SharedCard sharedCard){

			if (!string.IsNullOrEmpty (sharedCard.Card.FrontFileName)) {

				var userCard = new UserCard {
					Card = sharedCard.Card,
					CardId = sharedCard.Card.CardId
				};
				var frame = new RectangleF (LEFT_MARGIN + 5f, 10f, (float)UIScreen.MainScreen.Bounds.Width, LABEL_HEIGHT);

				AddCardImageButton (userCard, cell, 0);

				AddNameLabel(userCard, cell, ref frame);
				AddCompanyLabel (userCard, cell, ref frame);

			} else {
				var NameLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.NameLabel) as UILabel;
				if (NameLabel != null) {
					NameLabel.RemoveFromSuperview ();
				}

				var frame = new RectangleF (10f, 10f, 300f, 50f);
				NameLabel = new UILabel (frame);
				NameLabel.Tag = (int)Resources.UIElements.NameLabel;
				NameLabel.Text = sharedCard.Card.Name;
				NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 18f);

				cell.ContentView.AddSubview (NameLabel);
			}
		}
	}
}

