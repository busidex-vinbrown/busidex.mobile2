using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using Busidex.Presentation.iOS.Controllers;

namespace Busidex.Presentation.iOS
{
	public delegate void SaveSharedCardHandler(SharedCard sharedCard);

	public class SharedCardTableSource : BaseTableSource
	{

		public event SaveSharedCardHandler CardShared;

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
			return !SharedCards.Any () ? BASE_CELL_HEIGHT * 3 : BASE_CELL_HEIGHT + LABEL_HEIGHT * 3;
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

		protected void SaveShareCard(SharedCard sharedCard){
			if(CardShared != null){
				CardShared (sharedCard);
			}
		}

		void AddAcceptDeclineButtons(SharedCard card, UITableViewCell cell){

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
				card.Accepted = true;
				SaveShareCard(card);
				acceptedImg.Hidden = false;
				declinedImg.Hidden = true;
				acceptButton.Hidden = declineButton.Hidden = true;
			};

			declineButton.TouchUpInside += delegate(object sender, EventArgs e) {
				card.Declined = true;
				SaveShareCard(card);
				acceptedImg.Hidden = true;
				declinedImg.Hidden = false;
				acceptButton.Hidden = declineButton.Hidden = true;
			};
		}

		protected void AddPersonalMessageLabel(SharedCard card, UITableViewCell cell, ref RectangleF frame){
			var needsMessageLabel = false;

			frame.Height = LABEL_HEIGHT * 3;

			var MessageLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.PersonalMessageLabel) as UILabel;
			if (MessageLabel == null) {
				MessageLabel = new UILabel (frame);
				needsMessageLabel = true;
			}else{
				MessageLabel.Frame = frame;
			}

			if (!string.IsNullOrWhiteSpace (card.Recommendation)) {

				MessageLabel.Tag = (int)Resources.UIElements.PersonalMessageLabel;
				MessageLabel.Text = card.Recommendation;

//				nfloat width = frame.Size.Width -20f;
//
//				CoreGraphics.CGSize size=((NSString)MessageLabel.Text).StringSize(MessageLabel.Font,constrainedToSize:new CoreGraphics.CGSize(width,100f),lineBreakMode:UILineBreakMode.WordWrap);
//				var cgFrame = new CoreGraphics.CGRect(frame.Location, size);
//		
//				MessageLabel.Frame = cgFrame;

				MessageLabel.Hidden = false;
				MessageLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);
				MessageLabel.Lines = int.Parse((MessageLabel.Text.Length / 40).ToString()) + 1;;
				MessageLabel.LineBreakMode = UILineBreakMode.WordWrap;

				frame.Y += LABEL_HEIGHT * 2;
				if (needsMessageLabel) {
					cell.ContentView.AddSubview (MessageLabel);
				}
			} else {
				if (MessageLabel != null) {
					MessageLabel.RemoveFromSuperview ();
				}
			}
		}

		void AddControls(UITableViewCell cell, SharedCard sharedCard){

			if (!string.IsNullOrEmpty (sharedCard.Card.FrontFileName)) {

				var userCard = new UserCard {
					Card = sharedCard.Card,
					CardId = sharedCard.Card.CardId
				};
				var frame = new RectangleF (LEFT_MARGIN + 5f, 10f, (float)UIScreen.MainScreen.Bounds.Width - LEFT_MARGIN, LABEL_HEIGHT);

				AddNameLabel(userCard, cell, ref frame);
				AddCompanyLabel (userCard, cell, ref frame);
				AddAcceptDeclineButtons (sharedCard, cell);
				AddCardImageButton (userCard, cell, 0, false);

				frame.Y += sharedCard.Card.FrontOrientation == "H" 
					? CARD_HEIGHT_HORIZONTAL
					: CARD_HEIGHT_VERTICAL;

				AddPersonalMessageLabel (sharedCard, cell, ref frame);
			} 
		}
	}
}

