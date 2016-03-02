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
	public delegate void EditNotesHandler();
	public delegate void CardAddedToMyBusidexHandler(UserCard card);
	public delegate void CardRemovedFromMyBusidexHandler(UserCard card);
	public delegate void CallingPhoneNumberHandler();
	public delegate void SendingEmailHandler(string email);
	public delegate void ViewWebsiteHandler(string url);
	public delegate void SharingCardHandler();

	public class TableSource : BaseTableSource {

		public event EditNotesHandler EditingNotes;
		public event CallingPhoneNumberHandler CallingPhoneNumber;
		public event SendingEmailHandler SendingEmail;
		public event ViewWebsiteHandler ViewWebsite;
		public event CardAddedToMyBusidexHandler CardAddedToMyBusidex;
		public event CardRemovedFromMyBusidexHandler CardRemovedFromMyBusidex;
		public event SharingCardHandler SharingCard;

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
						: BASE_CELL_HEIGHT + 50f;
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
			string name = Resources.GA_LABEL_NOTES;
			if(SelectedCard != null && SelectedCard.Card != null){
				name = string.IsNullOrEmpty(SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_NOTES, name, 0);
		}

		protected void ShowPhoneNumbers(int idx){
			SelectedCard = Cards [idx];
			if (CallingPhoneNumber != null){
				CallingPhoneNumber ();
			}
			string name = Resources.GA_LABEL_PHONE;
			if(SelectedCard != null && SelectedCard.Card != null){
				name = string.IsNullOrEmpty(SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_PHONE, name, 0);
		}

		protected void SendEmail(string email){

			if (SendingEmail != null){
				SendingEmail (email);
				var card = Cards.SingleOrDefault (c => c.Card.Email != null && c.Card.Email.Equals (email));
				if (card != null) {
					ActivityController.SaveActivity ((long)EventSources.Email, card.CardId, userToken);
				}
				string name = Resources.GA_LABEL_EMAIL;

				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_EMAIL, name, 0);
			}
		}

		protected void ShowBrowser(string url){

			if (ViewWebsite != null){
				ViewWebsite (url.Replace ("http://", "").Replace ("https://", ""));
				var card = Cards.SingleOrDefault (c => c.Card.Url != null && c.Card.Url.Equals (url));
				if (card != null) {

					string name = Resources.GA_LABEL_URL;
					if(SelectedCard != null && SelectedCard.Card != null){
						name = string.IsNullOrEmpty(card.Card.Name) ? card.Card.CompanyName : card.Card.Name;
					}

					AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_URL, name, 0);

					ActivityController.SaveActivity ((long)EventSources.Website, card.CardId, userToken);
				}
			}
		}

		protected void ShareCard(int idx){
			SelectedCard = Cards [idx];
			if(SharingCard != null){
				SharingCard ();
			}
			string name = Resources.GA_LABEL_SHARE;
			if(SelectedCard != null && SelectedCard.Card != null){
				name = string.IsNullOrEmpty(SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_SHARE, name, 0);
		}

		static void toggleAddRemoveButtons(bool cardInMyBusidex, UITableViewCell cell){
			var panel = cell.ContentView.Subviews.SingleOrDefault (v => v.Tag == (int)Resources.UIElements.ButtonPanel);
			if(panel != null){
				var removeFromMyBusidexButton = panel.Subviews.SingleOrDefault (v => v.Tag == (int)Resources.UIElements.RemoveFromMyBusidexButton);
				var addToMyBusidexButton = panel.Subviews.SingleOrDefault (v => v.Tag == (int)Resources.UIElements.AddToMyBusidexButton);
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
					string name = Resources.GA_LABEL_ADD;
					if(userCard != null && userCard.Card != null){
						name = string.IsNullOrEmpty(userCard.Card.Name) ? userCard.Card.CompanyName : userCard.Card.Name;
					}

					AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_ADD, name, 0);
				}
			}
		}

		protected void RemoveFromMyBusidex(UserCard userCard, UITableViewCell cell){

			BaseController.ShowAlert ("Remove Card", "Remove this card from your Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if(button.Result == 0){
					InvokeOnMainThread( () => {
						using (NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME)) {
							if (cookie != null) {
								var ctrl = new Busidex.Mobile.MyBusidexController ();
								ctrl.RemoveFromMyBusidex (userCard.Card.CardId, cookie.Value);

								toggleAddRemoveButtons (true, cell);

								if (CardRemovedFromMyBusidex != null) {
									CardRemovedFromMyBusidex (userCard);
								}

								string name = Resources.GA_LABEL_REMOVED;
								if(userCard != null && userCard.Card != null){
									name = string.IsNullOrEmpty(userCard.Card.Name) ? userCard.Card.CompanyName : userCard.Card.Name;
								}

								AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_REMOVED, name, 0);
							}
						}
					});
				}
			});
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

							string name = Resources.GA_LABEL_MAP;
							if(card != null && card.Card != null){
								name = string.IsNullOrEmpty(card.Card.Name) ? card.Card.CompanyName : card.Card.Name;
							}

							AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_MAP, name, 0);
						});	
					};
						
					MapButton.Tag = (int)Resources.UIElements.MapButton;
					MapButton.SetBackgroundImage (UIImage.FromBundle ("maps.png"), UIControlState.Normal);

					FeatureButtons.Add (MapButton);
				}
			}
		}

		protected void AddNotesButton(ref List<UIButton> FeatureButtons, int idx){

			var NotesButton = UIButton.FromType (UIButtonType.System);
				
			NotesButton.SetBackgroundImage (UIImage.FromBundle ("notes.png"), UIControlState.Normal);
			NotesButton.Tag = (int)Resources.UIElements.NotesButton;
			NotesButton.TouchUpInside += delegate {
				EditNotes(idx);
			};

			FeatureButtons.Add (NotesButton);
		}

		protected void AddAddToMyBusidexButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			var addToMyBusidexButton = UIButton.FromType (UIButtonType.System);

			addToMyBusidexButton.SetBackgroundImage (UIImage.FromBundle ("add.png"), UIControlState.Normal);
			addToMyBusidexButton.Tag = (int)Resources.UIElements.AddToMyBusidexButton;
			addToMyBusidexButton.TouchUpInside += delegate {
				AddToMyBusidex (card, cell);
			};
			addToMyBusidexButton.Hidden = card.ExistsInMyBusidex;

			FeatureButtons.Add (addToMyBusidexButton);
		}

		protected void AddRemoveFromMyBusidexButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){

			var removeFromMyBusidexButton = UIButton.FromType (UIButtonType.System);

			removeFromMyBusidexButton.SetBackgroundImage (UIImage.FromBundle ("remove.png"), UIControlState.Normal);
			removeFromMyBusidexButton.Tag = (int)Resources.UIElements.RemoveFromMyBusidexButton;
			removeFromMyBusidexButton.TouchUpInside += delegate {
				RemoveFromMyBusidex (card, cell);
			};
			removeFromMyBusidexButton.Hidden = !card.ExistsInMyBusidex;

			FeatureButtons.Add (removeFromMyBusidexButton);
		}

		protected void AddEmailButton(UserCard card, ref List<UIButton> FeatureButtons){

			if(!string.IsNullOrEmpty(card.Card.Email)){
				var EmailButton = UIButton.FromType (UIButtonType.System);

				EmailButton.SetBackgroundImage (UIImage.FromBundle ("email-icon.png"), UIControlState.Normal);
				EmailButton.Tag = (int)Resources.UIElements.EmailButton;

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
				WebsiteButton.Tag = (int)Resources.UIElements.WebsiteButton;

				WebsiteButton.TouchUpInside += delegate {
					ShowBrowser(card.Card.Url);
				};

				FeatureButtons.Add (WebsiteButton);
			}
		}

		protected void AddPhoneButton(ref List<UIButton> FeatureButtons, int idx){

			var PhoneButton = UIButton.FromType (UIButtonType.System);

			PhoneButton.SetBackgroundImage (UIImage.FromBundle ("phone.png"), UIControlState.Normal);
			PhoneButton.Tag = (int)Resources.UIElements.PhoneNumberButton;

			PhoneButton.TouchUpInside += delegate {
				ShowPhoneNumbers(idx);
			};

			FeatureButtons.Add (PhoneButton);
		}

		protected void AddTextMessageButton(ref List<UIButton> FeatureButtons, int idx){

			var TextMessageButton = UIButton.FromType (UIButtonType.System);

			TextMessageButton.SetBackgroundImage (UIImage.FromBundle ("textmessage.png"), UIControlState.Normal);
			TextMessageButton.Tag = (int)Resources.UIElements.TextMessageButton;

			TextMessageButton.TouchUpInside += delegate {
				ShowPhoneNumbers(idx);
			};

			FeatureButtons.Add (TextMessageButton);
		}

		protected void AddShareCardButton(ref List<UIButton> FeatureButtons, int idx){

			var shareCardButton = UIButton.FromType (UIButtonType.System);

			shareCardButton.SetBackgroundImage (UIImage.FromBundle ("share.png"), UIControlState.Normal);
			shareCardButton.Tag = (int)Resources.UIElements.ShareCardButton;

			shareCardButton.TouchUpInside += delegate {
				ShareCard(idx);
			};

			FeatureButtons.Add (shareCardButton);
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
				AddShareCardButton (ref FeatureButtonList, idx);

				AddAddToMyBusidexButton (card, cell, ref FeatureButtonList);
				AddRemoveFromMyBusidexButton (card, cell, ref FeatureButtonList);

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