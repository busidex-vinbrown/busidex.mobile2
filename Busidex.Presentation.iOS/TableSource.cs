using System;
using Foundation;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.Drawing;
using System.Linq;
using System.IO;
using Busidex.Mobile;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public delegate void CardSelected();
	public delegate void EditNotesHandler();
	public delegate void CardAddedToMyBusidexHandler(UserCard card);
	public delegate void CallingPhoneNumberHandler();
	public delegate void SendingEmailHandler(string email);
	public delegate void ViewWebsiteHandler(string url);

	public class TableSource : BaseTableSource {

		public event CardSelected CardSelected;
		public event EditNotesHandler EditingNotes;
		public event CallingPhoneNumberHandler CallingPhoneNumber;
		public event SendingEmailHandler SendingEmail;
		public event ViewWebsiteHandler ViewWebsite;
		public event CardAddedToMyBusidexHandler CardAddedToMyBusidex;

		public UserCard SelectedCard{ get; set; }
		protected List<UserCard> Cards{ get; set; }
		protected bool NoCards;
		protected string userToken;

		public bool IsFiltering{ get; set;}
		public bool ShowNoCardMessage{ get; set; }
		public string NoCardsMessage{ get; set;}
		public bool ShowNotes{ get; set;}

		protected UIColor CELL_BACKGROUND_COLOR = UIColor.FromRGB (240, 239, 243);
		protected const float LEFT_MARGIN = 5F;
		protected const float CARD_HEIGHT_VERTICAL = 170f;
		protected const float CARD_HEIGHT_HORIZONTAL = 100f;
		protected const float CARD_WIDTH_VERTICAL = 110f;
		protected const float CARD_WIDTH_HORIZONTAL = 140f;
		protected const float SUB_LABEL_FONT_SIZE = 17f;
		protected const float LABEL_HEIGHT = 30f;
		protected const float LABEL_WIDTH = 170f;
		protected const float FEATURE_BUTTON_HEIGHT = 40f;
		protected const float FEATURE_BUTTON_WIDTH = 40f;
		protected const float FEATURE_BUTTON_MARGIN = 15f;
		protected const string NONE_MATCH_FILTER = "No cards match your filter";

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

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AuthenticationCookieName);

			if (cookie != null) {
				userToken = cookie.Value;
			}

		}
		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			tableView.BackgroundColor =	cell.ContentView.BackgroundColor = cell.BackgroundColor = CELL_BACKGROUND_COLOR;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}
			
		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{

			return NoCards ? BASE_CELL_HEIGHT * 3 : BASE_CELL_HEIGHT;
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

		protected async void GoToCard(int idx){
			SelectedCard = Cards [idx];
			if (CardSelected != null) {
				CardSelected ();
				await ActivityController.SaveActivity ((long)EventSources.Details, SelectedCard.Card.CardId, userToken);
			}
		}

		protected void EditNotes(int idx){
			SelectedCard = Cards [idx];
			if (EditingNotes != null){
				EditingNotes ();
			}
		}

		protected void ShowPhoneNumbers(int idx){
			SelectedCard = Cards [idx];
			if (CallingPhoneNumber != null){
				CallingPhoneNumber ();
			}
		}

		protected async void SendEmail(string email){

			if (SendingEmail != null){
				SendingEmail (email);
				var card = Cards.SingleOrDefault (c => c.Card.Email != null && c.Card.Email.Equals (email));
				if (card != null) {
					await ActivityController.SaveActivity ((long)EventSources.Email, card.CardId, userToken);
				}
			}
		}

		protected async void ShowBrowser(string url){

			if (ViewWebsite != null){
				ViewWebsite (url.Replace ("http://", "").Replace ("https://", ""));
				var card = Cards.SingleOrDefault (c => c.Card.Url != null && c.Card.Url.Equals (url));
				if (card != null) {
					await ActivityController.SaveActivity ((long)EventSources.Website, card.CardId, userToken);
				}
			}
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

		protected void AddToMyBusidex(UserCard userCard){

			using (NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AuthenticationCookieName)) {
				if (cookie != null) {
					var ctrl = new Busidex.Mobile.MyBusidexController ();
					ctrl.AddToMyBusidex (userCard.Card.CardId, cookie.Value);
					if (CardAddedToMyBusidex != null) {
						CardAddedToMyBusidex (userCard);
						ActivityController.SaveActivity ((long)EventSources.Add, userCard.Card.CardId, userToken);
					}
				}
			}
		}

		protected void AddMapButton(UserCard card, ref List<UIButton> FeatureButtons){
		
			var MapButton = UIButton.FromType (UIButtonType.System);

			if (card.Card.Addresses != null && card.Card.Addresses.Any ()) {
				string address = buildAddress (card.Card);

				if (!string.IsNullOrWhiteSpace (address)) {

					MapButton.TouchUpInside += delegate {

						InvokeOnMainThread (() => {
							var url = new NSUrl ("http://www.maps.google.com/?saddr=" + System.Net.WebUtility.UrlEncode (address.Trim ()));
							UIApplication.SharedApplication.OpenUrl (url);
							ActivityController.SaveActivity ((long)EventSources.Map, card.Card.CardId, userToken);
						});	
					};
						
					MapButton.Tag = (int)UIElements.MapButton;
					MapButton.SetBackgroundImage (UIImage.FromBundle ("maps.png"), UIControlState.Normal);

					FeatureButtons.Add (MapButton);
				}
			}
		}

		protected void AddNotesButton(ref List<UIButton> FeatureButtons, int idx){

			var NotesButton = UIButton.FromType (UIButtonType.System);
				
			NotesButton.SetBackgroundImage (UIImage.FromBundle ("notes.png"), UIControlState.Normal);
			NotesButton.Tag = (int)UIElements.NotesButton;
			NotesButton.TouchUpInside += delegate {
				EditNotes(idx);
			};

			FeatureButtons.Add (NotesButton);
		}

		protected void AddAddToMyBusidexButton(UserCard card, ref List<UIButton> FeatureButtons){

			if (!card.ExistsInMyBusidex) {
				var AddToMyBusidexButton = UIButton.FromType (UIButtonType.System);

				AddToMyBusidexButton.SetBackgroundImage (UIImage.FromBundle ("add.png"), UIControlState.Normal);
				AddToMyBusidexButton.Tag = (int)UIElements.AddToMyBusidexButton;
				AddToMyBusidexButton.TouchUpInside += delegate {
					AddToMyBusidex (card);
				};

				FeatureButtons.Add (AddToMyBusidexButton);
			}

//			bool needsAddButton = false;
//			//var AddToMyBusidexButton = cell.ContentView.Subviews.SingleOrDefault (s => s is UIButton && s.Tag == (int)UIElements.AddToMyBusidexButton) as UIButton;
//			if (AddToMyBusidexButton == null) {
//				AddToMyBusidexButton = UIButton.FromType (UIButtonType.System);
//				needsAddButton = true;
//			}
//
//			if (!card.ExistsInMyBusidex) {
//				AddToMyBusidexButton.SetTitle ("Add To My Busidex", UIControlState.Normal);
//				AddToMyBusidexButton.Hidden = card.ExistsInMyBusidex;
//				AddToMyBusidexButton.Tag = (int)UIElements.AddToMyBusidexButton;
//				AddToMyBusidexButton.Frame = frame;// new RectangleF (LEFT_MARGIN +, 100f, 120f, 22f);
//				AddToMyBusidexButton.Font = UIFont.FromName ("Helvetica", 14f);
//				AddToMyBusidexButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
//				AddToMyBusidexButton.SetTitleColor (UIColor.Blue, UIControlState.Normal);
//
//				var CheckMark = new UIImageView (new RectangleF (frame.X, frame.Y, 22f, 22f));
//				CheckMark.Image = UIImage.FromBundle ("checkmark.png");
//				CheckMark.Hidden = true;
//
//				AddToMyBusidexButton.TouchUpInside += delegate {
//					this.InvokeOnMainThread (() => {
//						AddToMyBusidexButton.Hidden = true;
//						CheckMark.Hidden = false;
//					});
//
//					AddToMyBusidex (card);
//				};
//
//				frame.Y += LABEL_HEIGHT;
//
//				if (needsAddButton) {
//					cell.ContentView.AddSubview (AddToMyBusidexButton);
//					cell.ContentView.AddSubview (CheckMark);
//				}
//			} else {
//				AddToMyBusidexButton.RemoveFromSuperview ();
//			}
		}

		protected void AddCardImageButton(UserCard card, UITableViewCell cell, int idx){

			bool needsCardImage;

			var CardImageButton = cell.ContentView.Subviews.SingleOrDefault (s => s is UIButton && s.Tag == (int)UIElements.CardImage) as UIButton;
			if (CardImageButton != null) {
				CardImageButton.RemoveFromSuperview ();
			}
			CardImageButton = new UIButton (UIButtonType.Custom);
			needsCardImage = CardImageButton.Tag <= 0;

			CardImageButton.Tag = (int)UIElements.CardImage;

			var fileName = Path.Combine (documentsPath, card.Card.FrontFileId + "." + card.Card.FrontType);

			if (File.Exists (fileName)) {
				CardImageButton.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal); 
			} else {
				CardImageButton.SetBackgroundImage (UIImage.FromBundle ("defaultUserImage.png"), UIControlState.Normal); 
			}

			CardImageButton.TouchUpInside += delegate {
				GoToCard (idx);
			};

			// Highlight the user's card
			if (card.Card.IsMyCard) {
				CardImageButton.Layer.BorderWidth = 2;
				CardImageButton.Layer.BorderColor = UIColor.Green.CGColor;
			}

			if (needsCardImage) {
				CardImageButton.Frame =
					card.Card.FrontOrientation == "H" 
					? new RectangleF (LEFT_MARGIN, 10f, CARD_WIDTH_HORIZONTAL, CARD_HEIGHT_HORIZONTAL)
					: new RectangleF (LEFT_MARGIN, 10f, CARD_WIDTH_VERTICAL, CARD_HEIGHT_VERTICAL);

				cell.ContentView.AddSubview (CardImageButton);
			}
		}

		protected void AddNameLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsNameLabel = false;
			var NameLabel = cell.ContentView.Subviews.SingleOrDefault(s=> s.Tag == (int)UIElements.NameLabel) as UILabel;

			if (NameLabel == null) {
				NameLabel = new UILabel (frame);
				needsNameLabel = true;
			}else{
				NameLabel.Frame = frame;
			}
			NameLabel.Tag = (int)UIElements.NameLabel;
			NameLabel.Text = string.IsNullOrEmpty(card.Card.Name) ? "(No Name)" : card.Card.Name;
			NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 16f);

			frame.Y += LABEL_HEIGHT;
			if (needsNameLabel) {
				cell.ContentView.AddSubview (NameLabel);
			}
		}

		protected void AddCompanyLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsCompanyLabel = false;

			var CompanyLabel = cell.ContentView.Subviews.SingleOrDefault (s => s.Tag == (int)UIElements.CompanyLabel) as UILabel;
			if (CompanyLabel == null) {
				CompanyLabel = new UILabel (frame);
				needsCompanyLabel = true;
			}else{
				CompanyLabel.Frame = frame;
			}

			if (!string.IsNullOrWhiteSpace (card.Card.CompanyName)) {

				CompanyLabel.Tag = (int)UIElements.CompanyLabel;
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

		protected void AddEmailButton(UserCard card, ref List<UIButton> FeatureButtons){

			if(!string.IsNullOrEmpty(card.Card.Email)){
				var EmailButton = UIButton.FromType (UIButtonType.System);

				EmailButton.SetBackgroundImage (UIImage.FromBundle ("email-icon.png"), UIControlState.Normal);
				EmailButton.Tag = (int)UIElements.EmailButton;

				EmailButton.TouchUpInside += delegate {
					SendEmail(card.Card.Email);
				};

				FeatureButtons.Add (EmailButton);
			}
		}

		protected void AddWebSiteButton(UserCard card, ref List<UIButton> FeatureButtons){

			if (!string.IsNullOrEmpty (card.Card.Url)) {
				var WebsiteButton = UIButton.FromType (UIButtonType.System);

				WebsiteButton.SetBackgroundImage (UIImage.FromBundle ("browser.png"), UIControlState.Normal);
				WebsiteButton.Tag = (int)UIElements.WebsiteButton;

				WebsiteButton.TouchUpInside += delegate {
					ShowBrowser(card.Card.Url);
				};

				FeatureButtons.Add (WebsiteButton);
			}
		}

		protected void AddPhoneButton(ref List<UIButton> FeatureButtons, int idx){

			var PhoneButton = UIButton.FromType (UIButtonType.System);

			PhoneButton.SetBackgroundImage (UIImage.FromBundle ("phone.png"), UIControlState.Normal);
			PhoneButton.Tag = (int)UIElements.PhoneNumberButton;

			PhoneButton.TouchUpInside += delegate {
				ShowPhoneNumbers(idx);
			};

			FeatureButtons.Add (PhoneButton);
		}
			
		public void AddControls(UITableViewCell cell, UserCard card, int idx){

			cell.Accessory = UITableViewCellAccessory.DetailButton;

			if (card != null && card.Card != null) {

				var FeatureButtonList = new List<UIButton> ();

				var frame = card.Card.FrontOrientation == "H" 
					? new RectangleF (CARD_WIDTH_HORIZONTAL + LEFT_MARGIN + 5f, 10f, LABEL_WIDTH, LABEL_HEIGHT)
					: new RectangleF (CARD_WIDTH_VERTICAL + LEFT_MARGIN + 5f, 10f, LABEL_WIDTH, LABEL_HEIGHT);

				var noCardLabel = cell.ContentView.Subviews.SingleOrDefault (v => v.Tag == -1);
				if(noCardLabel != null){
					noCardLabel.RemoveFromSuperview ();
				}
				AddCardImageButton (card, cell, idx);
				AddNameLabel (card, cell, ref frame);
				AddCompanyLabel (card, cell, ref frame);

				AddMapButton (card, ref FeatureButtonList);
				if (ShowNotes) {
					AddNotesButton (ref FeatureButtonList, idx);
				}
				AddEmailButton (card, ref FeatureButtonList);
				AddWebSiteButton (card, ref FeatureButtonList);
				AddPhoneButton (ref FeatureButtonList, idx);

				AddAddToMyBusidexButton (card, ref FeatureButtonList);

				AddFeatureButtons (cell, FeatureButtonList);
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