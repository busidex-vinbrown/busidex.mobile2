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
	public delegate void CardRemovedFromMyBusidexHandler(UserCard card);
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
		public event CardRemovedFromMyBusidexHandler CardRemovedFromMyBusidex;

		public UserCard SelectedCard{ get; set; }
		protected List<UserCard> Cards{ get; set; }
		public bool NoCards;
		protected string userToken;

		public bool ShowNotes{ get; set;}

		protected const float LEFT_MARGIN = 5f;
		protected const float CARD_HEIGHT_VERTICAL = 170f;
		protected const float CARD_HEIGHT_HORIZONTAL = 120f;
		protected const float CARD_WIDTH_VERTICAL = 110f;
		protected const float CARD_WIDTH_HORIZONTAL = 180f;
		protected const float SUB_LABEL_FONT_SIZE = 17f;
		protected const float LABEL_HEIGHT = 20f;
		protected const float LABEL_WIDTH = 170f;
		protected const float FEATURE_BUTTON_HEIGHT = 40f;
		protected const float FEATURE_BUTTON_WIDTH = 40f;
		protected const float FEATURE_BUTTON_MARGIN = 15f;


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
			if(NoCards){
				return BASE_CELL_HEIGHT;
			}else{
				UserCard card = Cards [indexPath.Row];
				return card.Card.FrontOrientation == "H"
					? BASE_CELL_HEIGHT
						: BASE_CELL_HEIGHT + 50f;
			}
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

		void toggleAddRemoveButtons(bool cardInMyBusidex, UITableViewCell cell){
			var panel = cell.ContentView.Subviews.SingleOrDefault (v => v.Tag == (int)UIElements.ButtonPanel);
			if(panel != null){
				var removeFromMyBusidexButton = panel.Subviews.SingleOrDefault (v => v.Tag == (int)UIElements.RemoveFromMyBusidexButton);
				var addToMyBusidexButton = panel.Subviews.SingleOrDefault (v => v.Tag == (int)UIElements.AddToMyBusidexButton);
				if(removeFromMyBusidexButton != null){
					removeFromMyBusidexButton.Hidden = cardInMyBusidex;
				}
				if(addToMyBusidexButton != null){
					addToMyBusidexButton.Hidden = !cardInMyBusidex;
				}
			}
		}

		protected void AddToMyBusidex(UserCard userCard, UITableViewCell cell){

			using (NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME)) {
				if (cookie != null) {
					var ctrl = new Busidex.Mobile.MyBusidexController ();
					ctrl.AddToMyBusidex (userCard.Card.CardId, cookie.Value);

					toggleAddRemoveButtons (false, cell);

					if (CardAddedToMyBusidex != null) {
						CardAddedToMyBusidex (userCard);
						ActivityController.SaveActivity ((long)EventSources.Add, userCard.Card.CardId, userToken);
					}
				}
			}
		}

		protected void RemoveFromMyBusidex(UserCard userCard, UITableViewCell cell){

			using (NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME)) {
				if (cookie != null) {
					var ctrl = new Busidex.Mobile.MyBusidexController ();
					ctrl.RemoveFromMyBusidex (userCard.Card.CardId, cookie.Value);

					toggleAddRemoveButtons (true, cell);

					if (CardRemovedFromMyBusidex != null) {
						CardRemovedFromMyBusidex (userCard);
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

		protected void AddAddToMyBusidexButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			var addToMyBusidexButton = UIButton.FromType (UIButtonType.System);

			addToMyBusidexButton.SetBackgroundImage (UIImage.FromBundle ("add.png"), UIControlState.Normal);
			addToMyBusidexButton.Tag = (int)UIElements.AddToMyBusidexButton;
			addToMyBusidexButton.TouchUpInside += delegate {
				AddToMyBusidex (card, cell);
			};
			addToMyBusidexButton.Hidden = card.ExistsInMyBusidex;

			FeatureButtons.Add (addToMyBusidexButton);
		}

		protected void AddRemoveFromMyBusidexButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			var removeFromMyBusidexButton = UIButton.FromType (UIButtonType.System);

			removeFromMyBusidexButton.SetBackgroundImage (UIImage.FromBundle ("remove.png"), UIControlState.Normal);
			removeFromMyBusidexButton.Tag = (int)UIElements.RemoveFromMyBusidexButton;
			removeFromMyBusidexButton.TouchUpInside += delegate {
				RemoveFromMyBusidex (card, cell);
			};
			removeFromMyBusidexButton.Hidden = !card.ExistsInMyBusidex;

			FeatureButtons.Add (removeFromMyBusidexButton);
		}

		protected void AddCardImageButton(UserCard card, UITableViewCell cell, int idx){


			var CardImageButton = cell.ContentView.Subviews.SingleOrDefault (s => s is UIButton && s.Tag == (int)UIElements.CardImage) as UIButton;
			if (CardImageButton != null) {
				CardImageButton.RemoveFromSuperview ();
			}
			CardImageButton = new UIButton (UIButtonType.Custom);

			CardImageButton.Tag = (int)UIElements.CardImage;

			var fileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName);

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

			const float CARD_TOP = (LABEL_HEIGHT * 2f) + 10f;

			CardImageButton.Frame =
				card.Card.FrontOrientation == "H" 
				? new RectangleF (LEFT_MARGIN, CARD_TOP, CARD_WIDTH_HORIZONTAL, CARD_HEIGHT_HORIZONTAL)
				: new RectangleF (LEFT_MARGIN, CARD_TOP, CARD_WIDTH_VERTICAL, CARD_HEIGHT_VERTICAL);

			cell.ContentView.AddSubview (CardImageButton);
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

				var frame = new RectangleF (LEFT_MARGIN + 5f, 10f, (float)UIScreen.MainScreen.Bounds.Width, LABEL_HEIGHT);

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

				AddAddToMyBusidexButton (card, cell, ref FeatureButtonList);
				AddRemoveFromMyBusidexButton (card, cell, ref FeatureButtonList);

				AddFeatureButtons (cell, FeatureButtonList);
			}
		}

		protected void GoToCard(int idx){
			SelectedCard = Cards [idx];
			if (CardSelected != null) {
				CardSelected ();
				ActivityController.SaveActivity ((long)EventSources.Details, SelectedCard.Card.CardId, userToken);
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