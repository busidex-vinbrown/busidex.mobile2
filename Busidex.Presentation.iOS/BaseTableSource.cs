﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;

namespace Busidex.Presentation.iOS
{
	public delegate void CardSelected (UserCard card, bool showNotes);

	public class BaseTableSource : UITableViewSource
	{
		protected const float BASE_CELL_HEIGHT = 200f;

		protected const float LEFT_MARGIN = 5f;
		protected const float LABEL_HEIGHT = 20f;
		protected const float LABEL_WIDTH = 170f;
		protected const float FEATURE_BUTTON_HEIGHT = 40f;
		protected const float FEATURE_BUTTON_WIDTH = 40f;
		protected const float FEATURE_BUTTON_MARGIN = 15f;
		protected const float CARD_HEIGHT_VERTICAL = 220f;
		protected const float CARD_HEIGHT_HORIZONTAL = 128f;
		protected const float CARD_WIDTH_VERTICAL = 128f;
		protected const float CARD_WIDTH_HORIZONTAL = 220f;
		protected const float SUB_LABEL_FONT_SIZE = 17f;
		protected const float SUB_SUB_LABEL_FONT_SIZE = 12f;
		protected const string NONE_MATCH_FILTER = "No cards match your filter";

		protected List<UserCard> Cards{ get; set; }

		public UserCard SelectedCard{ get; set; }

		readonly UIColor CELL_BACKGROUND_COLOR = UIColor.FromRGB (240, 239, 243);

		protected List<UITableViewCell> cellCache;

		public string NoCardsMessage{ get; set; }

		public bool ShowNoCardMessage{ get; set; }

		public bool IsFiltering{ get; set; }

		protected string documentsPath = Resources.DocumentsPath;

		public event CardSelected CardSelected;

		protected void LoadNoCardMessage (UITableViewCell cell)
		{

			const float LABEL_HEIGHT = 61f * 3;
			const float LABEL_WIDTH = 280f;

			var frame = new RectangleF (10f, 10f, LABEL_WIDTH, LABEL_HEIGHT);

			var lbl = new UILabel (frame);
			lbl.Text = IsFiltering ? NONE_MATCH_FILTER : NoCardsMessage;
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Font = UIFont.FromName ("Helvetica", 17f);
			lbl.Lines = 3;

			foreach (var view in cell.ContentView.Subviews) {
				view.RemoveFromSuperview ();
			}
			lbl.Tag = -1;
			cell.ContentView.AddSubview (lbl);

			cell.Frame = frame;

		}

		protected void GoToCard (int idx, bool showNotes)
		{
			SelectedCard = Cards [idx];
			if (CardSelected != null) {
				CardSelected (SelectedCard, showNotes);
				ActivityController.SaveActivity ((long)EventSources.Details, SelectedCard.Card.CardId, UISubscriptionService.AuthToken);
			}
		}

		protected void AddCardImageButton (UserCard card, UITableViewCell cell, int idx, bool showNotes)
		{


			var CardImageButton = cell.ContentView.Subviews.SingleOrDefault (s => s is UIButton && s.Tag == (int)Resources.UIElements.CardImage) as UIButton;
			if (CardImageButton != null) {
				CardImageButton.RemoveFromSuperview ();
			}
			CardImageButton = new UIButton (UIButtonType.Custom);

			CardImageButton.Tag = (int)Resources.UIElements.CardImage;

			var fileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName);

			if (File.Exists (fileName)) {
				CardImageButton.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal); 
			} else {
				CardImageButton.SetBackgroundImage (UIImage.FromBundle ("defaultUserImage.png"), UIControlState.Normal); 
			}

			CardImageButton.TouchUpInside += delegate {
				GoToCard (idx, showNotes);
			};

			// Highlight the user's card
			if (card.Card.IsMyCard) {
				CardImageButton.Layer.BorderWidth = 2;
				CardImageButton.Layer.BorderColor = UIColor.Green.CGColor;
			}

			const float CARD_TOP = (LABEL_HEIGHT * 2f) + 10f;

			CardImageButton.Frame =
				card.Card.FrontOrientation == "H" 
				? new RectangleF (LEFT_MARGIN, CARD_TOP, CARD_WIDTH_HORIZONTAL, CARD_HEIGHT_HORIZONTAL)
				: new RectangleF (LEFT_MARGIN, CARD_TOP, CARD_WIDTH_VERTICAL, CARD_HEIGHT_VERTICAL);

			cell.ContentView.AddSubview (CardImageButton);
		}

		protected void AddNameLabel (UserCard card, UITableViewCell cell, ref RectangleF frame)
		{
			var needsNameLabel = false;
			var NameLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.NameLabel) as UILabel;

			if (NameLabel == null) {
				NameLabel = new UILabel (frame);
				needsNameLabel = true;
			} else {
				NameLabel.Frame = frame;
			}
			NameLabel.Tag = (int)Resources.UIElements.NameLabel;
			NameLabel.Text = string.IsNullOrEmpty (card.Card.Name) ? "(No Name)" : card.Card.Name;
			NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 16f);

			frame.Y += LABEL_HEIGHT;
			if (needsNameLabel) {
				cell.ContentView.AddSubview (NameLabel);
			}
		}

		protected void AddCompanyLabel (UserCard card, UITableViewCell cell, ref RectangleF frame)
		{
			var needsCompanyLabel = false;

			var CompanyLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)Resources.UIElements.CompanyLabel) as UILabel;
			if (CompanyLabel == null) {
				CompanyLabel = new UILabel (frame);
				needsCompanyLabel = true;
			} else {
				CompanyLabel.Frame = frame;
			}

			if (!string.IsNullOrWhiteSpace (card.Card.CompanyName)) {

				CompanyLabel.Tag = (int)Resources.UIElements.CompanyLabel;
				CompanyLabel.Text = card.Card.CompanyName;
				CompanyLabel.Hidden = false;
				CompanyLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);

				frame.Y += LABEL_HEIGHT;
				if (needsCompanyLabel) {
					cell.ContentView.AddSubview (CompanyLabel);
				}
			} else {
				if (CompanyLabel != null) {
					CompanyLabel.RemoveFromSuperview ();
				}
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 1;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
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
	}
}

