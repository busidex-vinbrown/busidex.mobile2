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

		void AddAcceptDeclineButtons(UserCard card, UITableViewCell cell){

			const float IMAGE_TOP = 90f;
			const float IMAGE_LEFT = CARD_WIDTH_HORIZONTAL + 30f;

			var acceptedImg = new UIImageView (new RectangleF (IMAGE_LEFT, IMAGE_TOP, 25f, 25f));
			acceptedImg.Image = UIImage.FromBundle ("checkmark.png");
			acceptedImg.Tag = (int)Resources.UIElements.AcceptCard;
			acceptedImg.Hidden = true;
			cell.AddSubview (acceptedImg);

			var declinedImg = new UIImageView (new RectangleF (IMAGE_LEFT, IMAGE_TOP, 25f, 25f));
			declinedImg.Image = UIImage.FromBundle ("red_minus.png");
			declinedImg.Tag = (int)Resources.UIElements.DeclineCard;
			declinedImg.Hidden = true;
			cell.AddSubview (declinedImg);

			var frame = new RectangleF (CARD_WIDTH_HORIZONTAL + 10f, 70f, 120f, LABEL_HEIGHT);

			var acceptButton = UIButton.FromType (UIButtonType.System);
			acceptButton.Frame = frame;
			acceptButton.SetTitle ("Accept", UIControlState.Normal);

			cell.AddSubview (acceptButton);

			frame.Y += LABEL_HEIGHT + 20f;

			var declineButton = UIButton.FromType (UIButtonType.System);
			declineButton.Frame = frame;
			declineButton.SetTitle ("Decline", UIControlState.Normal);

			cell.AddSubview (declineButton);

			acceptButton.TouchUpInside += delegate(object sender, EventArgs e) {
				acceptedImg.Hidden = false;
				declinedImg.Hidden = true;
				acceptButton.Hidden = declineButton.Hidden = true;
			};

			declineButton.TouchUpInside += delegate(object sender, EventArgs e) {
				acceptedImg.Hidden = true;
				declinedImg.Hidden = false;
				acceptButton.Hidden = declineButton.Hidden = true;
			};
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
				AddAcceptDeclineButtons (userCard, cell);
			} 
		}
	}
}

