using System;
using Foundation;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.Drawing;
using System.Linq;
using Busidex.Mobile;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class TableSource : BaseTableSource {

		public bool NoCards;

		public bool ShowNotes{ get; set;}

		List<UserCard> TableItems;

		public TableSource (List<UserCard> items)
		{
			if (!items.Any ()) {
				NoCards = true;
				items.Add (new UserCard ());
			}
			TableItems = items;
			cellCache = new List<UITableViewCell> ();
			Cards = new List<UserCard> ();

			Cards.AddRange (items);

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			if (cookie != null) {
				userToken = cookie.Value;
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}
			
		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (NoCards || Cards == null || indexPath.Row >= Cards.Count) {
				return BASE_CELL_HEIGHT;
			} 

			UserCard card = Cards [indexPath.Row];
			if (card == null) {
				return BASE_CELL_HEIGHT;
			} 

			return card.Card.FrontOrientation == "H"
					? BASE_CELL_HEIGHT
						: CARD_HEIGHT_VERTICAL + 65f;
		}
			
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var card = TableItems [indexPath.Row];

			var cell = tableView.DequeueReusableCell (MyBusidexController.BusidexCellId, indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
			if (cellCache.All (c => c.Tag != indexPath.Row)) {
				cellCache.Add (cell);
			} 

			if (NoCards) {
				LoadNoCardMessage (cell);
			} else {
				AddControls (cell, card, indexPath.Row);
			}
			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}


		public void AddControls(UITableViewCell cell, UserCard card, int idx){

			if (card != null && card.Card != null) {

				var frame = new RectangleF (LEFT_MARGIN + 5f, 10f, (float)UIScreen.MainScreen.Bounds.Width, LABEL_HEIGHT);

				var noCardLabel = cell.ContentView.Subviews.SingleOrDefault (v => v.Tag == -1);
				if(noCardLabel != null){
					noCardLabel.RemoveFromSuperview ();
				}
				AddCardImageButton (card, cell, idx);
				AddNameLabel (card, cell, ref frame);
				AddCompanyLabel (card, cell, ref frame);
			}
		}
			
		protected string buildAddress(Card card){

			var address = string.Empty;
			address += string.IsNullOrEmpty(card.Addresses [0].Address1) ? string.Empty : card.Addresses [0].Address1;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].Address2) ? string.Empty : card.Addresses [0].Address2;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].City) ? string.Empty : card.Addresses [0].City;
			address += " ";
			address += card.Addresses [0].State == null ? string.Empty : card.Addresses [0].State.Code;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].ZipCode) ? string.Empty : card.Addresses [0].ZipCode;

			return address;
		}
	}
}