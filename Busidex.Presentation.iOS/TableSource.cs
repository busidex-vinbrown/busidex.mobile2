using System;
using Foundation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Busidex.Mobile.Models;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using MessageUI;
//using NewRelic;
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

	public class TableSource : UITableViewSource {

		public event CardSelected CardSelected;
		public event EditNotesHandler EditingNotes;
		public event CallingPhoneNumberHandler CallingPhoneNumber;
		public event SendingEmailHandler SendingEmail;
		public event ViewWebsiteHandler ViewWebsite;

		List<UserCard> TableItems;
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		public UserCard SelectedCard{ get; set; }
		protected List<UITableViewCell> cellCache;
		protected List<UserCard> Cards{ get; set; }
		protected bool NoCards;
		public bool IsFiltering{ get; set;}
		public bool ShowNoCardMessage{ get; set; }
		public string NoCardsMessage{ get; set;}
		public bool ShowNotes{ get; set;}
		public event CardAddedToMyBusidexHandler CardAddedToMyBusidex;
		protected UIColor CELL_BACKGROUND_COLOR = UIColor.FromRGB (240, 236, 236);
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
		protected string userToken;

		protected enum UIElements{
			CardImage = 1,
			NameLabel = 2,
			CompanyLabel = 3,
			AddToMyBusidexButton = 4,
			MapButton = 5,
			NotesButton = 6,
			EmailButton = 7,
			WebsiteButton = 8,
			PhoneNumberButton = 9
		}

		public TableSource (List<UserCard> items)
		{
			if (items.Count () == 0) {
				NoCards = true;
				items.Add (new UserCard ());
			}
			TableItems = items;
			cellCache = new List<UITableViewCell> ();
			Cards = new List<UserCard> ();

			Cards.AddRange (items);

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName).SingleOrDefault();

			if (cookie != null) {
				userToken = cookie.Value;
			}

		}
		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.ContentView.BackgroundColor = CELL_BACKGROUND_COLOR;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}
			
		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			float baseHeight = 190f;
			return NoCards ? baseHeight * 3 : baseHeight;
		}
			
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var card = (UserCard)TableItems [indexPath.Row];

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

		protected void GoToCard(int idx){
			this.SelectedCard = Cards[idx];
			if (this.CardSelected != null) {
				this.CardSelected ();
				//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.CARD_CLICKED, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
				ActivityController.SaveActivity ((long)EventSources.Details, this.SelectedCard.Card.CardId, userToken);
			}
		}

		protected void EditNotes(int idx){
			this.SelectedCard = Cards [idx];
			if (this.EditingNotes != null){
				this.EditingNotes ();
				//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.NOTES_VIEWED, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
			}
		}

		protected void ShowPhoneNumbers(int idx){
			this.SelectedCard = Cards [idx];
			if (this.CallingPhoneNumber != null){
				this.CallingPhoneNumber ();
				//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.PHONE_CALLED, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
			}
		}

		protected async void SendEmail(string email){

			if (this.SendingEmail != null){
				this.SendingEmail (email);
				//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.EMAIL_SENT, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
				var card = Cards.Where (c => c.Card.Email != null && c.Card.Email.Equals (email)).SingleOrDefault ();
				if (card != null) {
					ActivityController.SaveActivity ((long)EventSources.Email, card.CardId, userToken);
				}
			}
		}

		protected async void ShowBrowser(string url){

			if (this.ViewWebsite != null){
				this.ViewWebsite (url.Replace("http://", "").Replace("https://",""));
				//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.WEBSITE_VISIT, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
				var card = Cards.Where (c => c.Card.Url != null && c.Card.Url.Equals (url)).SingleOrDefault ();
				if (card != null) {
					ActivityController.SaveActivity ((long)EventSources.Website, card.CardId, userToken);
				}
			}
		}

		protected void LoadNoCardMessage(UITableViewCell cell){
		
			float labelHeight = 61f * 3;
			float labelWidth = 280f;

			var frame = new RectangleF (10f, 10f, labelWidth, labelHeight);

			UILabel lbl = new UILabel (frame);
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

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName).SingleOrDefault();
			if (cookie != null) {
				Busidex.Mobile.MyBusidexController ctrl = new Busidex.Mobile.MyBusidexController ();
				ctrl.AddToMyBusidex (userCard.Card.CardId, cookie.Value);
				if (CardAddedToMyBusidex != null) {
					CardAddedToMyBusidex (userCard);
					//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.CARD_ADDED, "UI Interactions", new NSNumber (1));
					ActivityController.SaveActivity ((long)EventSources.Add, userCard.Card.CardId, userToken);
				}
			}
		}

		protected void AddMapButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){
		
			var MapButton = UIButton.FromType (UIButtonType.System);

			if (card.Card.Addresses != null && card.Card.Addresses.Count () > 0) {
				string address = buildAddress (card.Card);

				if (!string.IsNullOrWhiteSpace (address)) {

					MapButton.TouchUpInside += delegate {

						this.InvokeOnMainThread( ()=>{
							var url = new NSUrl("http://www.maps.google.com/?saddr=" + System.Net.WebUtility.UrlEncode(address.Trim()));
							UIApplication.SharedApplication.OpenUrl(url);
							//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.CARD_MAPPED, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
							ActivityController.SaveActivity ((long)EventSources.Map, card.Card.CardId, userToken);
						});	
					};
						
					MapButton.Tag = (int)UIElements.MapButton;
					MapButton.SetBackgroundImage (UIImage.FromBundle ("maps.png"), UIControlState.Normal);

					FeatureButtons.Add (MapButton);
				}
			}
		}

		protected void AddNotesButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons, int idx){

			var NotesButton = UIButton.FromType (UIButtonType.System);
				
			NotesButton.SetBackgroundImage (UIImage.FromBundle ("notes.png"), UIControlState.Normal);
			NotesButton.Tag = (int)UIElements.NotesButton;
			NotesButton.TouchUpInside += delegate {
				EditNotes(idx);
			};

			FeatureButtons.Add (NotesButton);
		}

		protected void AddAddToMyBusidexButton(UserCard card, UITableViewCell cell, ref RectangleF frame){
			bool needsAddButton = false;
			var AddToMyBusidexButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == (int)UIElements.AddToMyBusidexButton).SingleOrDefault () as UIButton;
			if (AddToMyBusidexButton == null) {
				AddToMyBusidexButton = UIButton.FromType (UIButtonType.System);
				needsAddButton = true;
			}

			if (!card.ExistsInMyBusidex) {
				AddToMyBusidexButton.SetTitle ("Add To My Busidex", UIControlState.Normal);
				AddToMyBusidexButton.Hidden = card.ExistsInMyBusidex;
				AddToMyBusidexButton.Tag = (int)UIElements.AddToMyBusidexButton;
				AddToMyBusidexButton.Frame = frame;// new RectangleF (LEFT_MARGIN +, 100f, 120f, 22f);
				AddToMyBusidexButton.Font = UIFont.FromName ("Helvetica", 14f);
				AddToMyBusidexButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
				AddToMyBusidexButton.SetTitleColor (UIColor.Blue, UIControlState.Normal);

				var CheckMark = new UIImageView (new RectangleF (frame.X, frame.Y, 22f, 22f));
				CheckMark.Image = UIImage.FromBundle ("checkmark.png");
				CheckMark.Hidden = true;

				AddToMyBusidexButton.TouchUpInside += delegate {
					this.InvokeOnMainThread (() => {
						AddToMyBusidexButton.Hidden = true;
						CheckMark.Hidden = false;
					});

					AddToMyBusidex (card);
				};

				frame.Y += LABEL_HEIGHT;

				if (needsAddButton) {
					cell.ContentView.AddSubview (AddToMyBusidexButton);
					cell.ContentView.AddSubview (CheckMark);
				}
			} else {
				AddToMyBusidexButton.RemoveFromSuperview ();
			}
		}

		protected void AddCardImageButton(UserCard card, UITableViewCell cell, int idx){

			bool needsCardImage = false;

			var CardImageButton = cell.ContentView.Subviews.SingleOrDefault (s => s is UIButton && s.Tag == (int)UIElements.CardImage) as UIButton;
			if (CardImageButton != null) {
				CardImageButton.RemoveFromSuperview ();
			}
			CardImageButton = new UIButton (UIButtonType.Custom);
			needsCardImage = CardImageButton.Tag <= 0;

			CardImageButton.Tag = (int)UIElements.CardImage;

			var fileName = System.IO.Path.Combine (documentsPath, card.Card.FrontFileId + "." + card.Card.FrontType);

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

		protected void AddEmailButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			if(!string.IsNullOrEmpty(card.Card.Email)){
				var EmailButton = UIButton.FromType (UIButtonType.System);

				EmailButton.SetBackgroundImage (UIImage.FromBundle ("email-icon.png"), UIControlState.Normal);
				EmailButton.Tag = (int)UIElements.EmailButton;

				FeatureButtons.Add (EmailButton);

				EmailButton.TouchUpInside += delegate {
					SendEmail(card.Card.Email);
				};
			}
		}

		protected void AddWebSiteButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			if (!string.IsNullOrEmpty (card.Card.Url)) {
				var WebsiteButton = UIButton.FromType (UIButtonType.System);

				WebsiteButton.SetBackgroundImage (UIImage.FromBundle ("browser.png"), UIControlState.Normal);
				WebsiteButton.Tag = (int)UIElements.WebsiteButton;

				FeatureButtons.Add (WebsiteButton);

				WebsiteButton.TouchUpInside += delegate {
					ShowBrowser(card.Card.Url);
				};
			}
		}

		protected void AddPhoneButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons, int idx){

			var PhoneButton = UIButton.FromType (UIButtonType.System);

			PhoneButton.SetBackgroundImage (UIImage.FromBundle ("phone.png"), UIControlState.Normal);
			PhoneButton.Tag = (int)UIElements.PhoneNumberButton;

			FeatureButtons.Add (PhoneButton);

			PhoneButton.TouchUpInside += delegate {

				ShowPhoneNumbers(idx);
			};
		}

		protected void AddFeatureButtons(UserCard card, UITableViewCell cell, List<UIButton> FeatureButtons){

			bool isVertical = card.Card.FrontOrientation == "V"; 

			float buttonY = 110f + LABEL_HEIGHT;
			if(isVertical){
				buttonY -= FEATURE_BUTTON_HEIGHT;
			}
			float buttonX =  isVertical ? CARD_WIDTH_VERTICAL + LEFT_MARGIN: LEFT_MARGIN;
				
			var cellButtons = cell.ContentView.Subviews.Where (s => s.Tag > (int)UIElements.AddToMyBusidexButton).ToList ();
			foreach(var button in cellButtons){
				button.RemoveFromSuperview ();
			}
				
			var frame = new RectangleF (buttonX, buttonY, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
			float buttonXOriginal = buttonX;
			int idx = 0;
			foreach(var button in FeatureButtons.OrderBy(b=>b.Tag)){
			
				button.Frame = frame;
				cell.ContentView.AddSubview (button);

				idx++;
				if (idx % 3 == 0 && isVertical) { 
					buttonX = buttonXOriginal;
					frame.Y += FEATURE_BUTTON_HEIGHT + 10f;
				} else {
					buttonX += FEATURE_BUTTON_WIDTH + FEATURE_BUTTON_MARGIN;
				}
				frame.X = buttonX;

			}
		}

		public void AddControls(UITableViewCell cell, UserCard card, int idx){

			if (card != null && card.Card != null) {

				var frame = card.Card.FrontOrientation == "H" 
					? new RectangleF (CARD_WIDTH_HORIZONTAL + LEFT_MARGIN + 5f, 10f, LABEL_WIDTH, LABEL_HEIGHT)
					: new RectangleF (CARD_WIDTH_VERTICAL + LEFT_MARGIN + 5f, 10f, LABEL_WIDTH, LABEL_HEIGHT);

				var noCardLabel = cell.ContentView.Subviews.Where (v => v.Tag == -1).SingleOrDefault ();
				if(noCardLabel != null){
					noCardLabel.RemoveFromSuperview ();
				}
				AddCardImageButton (card, cell, idx);
				AddNameLabel (card, cell, ref frame);
				AddCompanyLabel (card, cell, ref frame);
				AddAddToMyBusidexButton (card, cell, ref frame);

				var FeatureButtonList = new List<UIButton> ();

				AddMapButton (card, cell, ref FeatureButtonList);
				AddEmailButton (card, cell, ref FeatureButtonList);
				AddWebSiteButton (card, cell, ref FeatureButtonList);
				AddPhoneButton (card, cell, ref FeatureButtonList, idx);
				if (this.ShowNotes) {
					AddNotesButton (card, cell, ref FeatureButtonList, idx);
				}
				AddFeatureButtons (card, cell, FeatureButtonList);
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
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