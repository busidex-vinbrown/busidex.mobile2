// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using MessageUI;
using System.Linq;

namespace Busidex.Presentation.iOS
{
	public partial class ButtonPanelController : BaseController
	{
		public UserCard SelectedCard{ get; set; }

		string FrontFileName{ get; set; }

		public ButtonPanelController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnAdd.TouchUpInside += delegate {
				AddToMyBusidex ();
			};

			btnRemove.TouchUpInside += delegate {
				RemoveCardFromMyBusidex (SelectedCard);
			};

			btnShare.TouchUpInside += delegate {
				ShareCard (SelectedCard);	
			};

			btnEmail.TouchUpInside += delegate {
				if (!string.IsNullOrEmpty (SelectedCard.Card.Email)) {
					SendEmail ();
				}
			};

			btnBrowser.TouchUpInside += delegate {
				if (!string.IsNullOrEmpty (SelectedCard.Card.Url)) {
					OpenBrowser ();
				}
			};

			btnNotes.TouchUpInside += delegate {
				if (SelectedCard.ExistsInMyBusidex) {
					EditNotes ();
				}
			};

			btnPhone.TouchUpInside += delegate {
				if (SelectedCard.Card.PhoneNumbers != null && SelectedCard.Card.PhoneNumbers.Any ()) {
					ShowPhoneNumbers ();
				}
			};

			btnMaps.TouchUpInside += delegate {
				if (SelectedCard.Card.Addresses != null && SelectedCard.Card.Addresses.Any ()) {
					ShowMaps ();
				}
			};

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			try {
				LoadCard ();
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}

			// Update from the subscription service if this has been updated
			SelectedCard.ExistsInMyBusidex = UISubscriptionService.ExistsInMyBusidex (SelectedCard);

			btnAdd.Hidden =	SelectedCard.ExistsInMyBusidex;
			btnRemove.Hidden = !SelectedCard.ExistsInMyBusidex;
			btnEmail.Enabled = !string.IsNullOrEmpty (SelectedCard.Card.Email);	
			btnBrowser.Enabled = !string.IsNullOrEmpty (SelectedCard.Card.Url);
			btnNotes.Enabled = SelectedCard.ExistsInMyBusidex;
			btnPhone.Enabled = SelectedCard.Card.PhoneNumbers != null && SelectedCard.Card.PhoneNumbers.Any ();	
			btnMaps.Enabled = SelectedCard.Card.Addresses != null && SelectedCard.Card.Addresses.Any ();
		}

		void LoadCard ()
		{

			if (SelectedCard != null && SelectedCard.Card != null) {

				BusinessCardDimensions dimensions = GetCardDimensions (SelectedCard.Card.FrontOrientation);
				btnCard.Frame = new CoreGraphics.CGRect (dimensions.MarginLeft, 75f, dimensions.Width, dimensions.Height);

				FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				if (File.Exists (FrontFileName)) {
					btnCard.SetBackgroundImage (UIImage.FromFile (FrontFileName), UIControlState.Normal);
					btnCard.Layer.AddSublayer (GetBorder (btnCard.Frame, UIColor.Gray.CGColor));
				}

				btnCard.TouchUpInside -= goToCard;
				btnCard.TouchUpInside += goToCard;
			}
		}

		void goToCard (object sender, EventArgs e)
		{
			var cardDetailController = Storyboard.InstantiateViewController ("CardDetailController") as CardDetailController;
			cardDetailController.SelectedCard = SelectedCard;

			if (cardDetailController != null) {
				NavigationController.PushViewController (cardDetailController, true);
			}
		}

		void ToggleAddRemoveButtons (bool cardInMyBusidex)
		{
			btnRemove.Hidden = cardInMyBusidex;
			btnAdd.Hidden = !cardInMyBusidex;
		}

		void AddToMyBusidex ()
		{
			UISubscriptionService.AddCardToMyBusidex (SelectedCard);
			ToggleAddRemoveButtons (false);

			#region Event Tracking
			string name = Resources.GA_LABEL_ADD;
			if (SelectedCard != null && SelectedCard.Card != null) {
				name = string.IsNullOrEmpty (SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_ADD, name, 0);

			ActivityController.SaveActivity ((long)EventSources.Add, SelectedCard.Card.CardId, UISubscriptionService.AuthToken);
			#endregion
		}

		void RemoveCardFromMyBusidex (UserCard userCard)
		{

			Application.ShowAlert ("Remove Card", "Remove this card from your Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if (button.Result == 0) {

					UISubscriptionService.RemoveCardFromMyBusidex (userCard);

					#region Event Tracking
					string name = Resources.GA_LABEL_REMOVED;
					if (SelectedCard != null && SelectedCard.Card != null) {
						name = string.IsNullOrEmpty (SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
					}

					AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_REMOVED, name, 0);
					#endregion

					InvokeOnMainThread (() => ToggleAddRemoveButtons (true));

				}
			});
		}

		void OpenBrowser ()
		{

			if (string.IsNullOrEmpty (SelectedCard.Card.Url)) {
				return;
			}

			var url = SelectedCard.Card.Url.Replace ("http://", "");
			try {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url));

				string name = Resources.GA_LABEL_URL;
				if (SelectedCard != null && SelectedCard.Card != null) {
					name = string.IsNullOrEmpty (SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
				}
				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_URL, name, 0);

				ActivityController.SaveActivity ((long)EventSources.Website, SelectedCard.CardId, UISubscriptionService.AuthToken);

			} catch (Exception ex) {
				Application.ShowAlert ("Could not open this website", string.Format ("There appears to be a problem with the website address: {0}.", SelectedCard.Card.Url), "Ok");
				Xamarin.Insights.Report (ex);
			}
		}

		void SendEmail ()
		{
			var _mailController = new MFMailComposeViewController ();
			_mailController.SetToRecipients (new []{ SelectedCard.Card.Email });

			_mailController.Finished += ( s, args) => InvokeOnMainThread (
				() => args.Controller.DismissViewController (true, null)
			);
			PresentViewController (_mailController, true, null);

			const string name = Resources.GA_LABEL_EMAIL;

			ActivityController.SaveActivity ((long)EventSources.Email, SelectedCard.CardId, UISubscriptionService.AuthToken);

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_EMAIL, name, 0);
		}

		void EditNotes ()
		{

			var notesController = Storyboard.InstantiateViewController ("NotesController") as NotesController;
			notesController.SelectedCard = SelectedCard;

			if (notesController != null) {
				NavigationController.PushViewController (notesController, true);
			}

			string name = Resources.GA_LABEL_NOTES;
			if (notesController.SelectedCard != null && notesController.SelectedCard.Card != null) {
				name = string.IsNullOrEmpty (notesController.SelectedCard.Card.Name) ? notesController.SelectedCard.Card.CompanyName : notesController.SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_NOTES, name, 0);
		}

		void ShowPhoneNumbers ()
		{
			var phoneViewController = Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.SelectedCard = SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}

			string name = Resources.GA_LABEL_PHONE;
			if (phoneViewController.SelectedCard != null && phoneViewController.SelectedCard.Card != null) {
				name = string.IsNullOrEmpty (phoneViewController.SelectedCard.Card.Name) ? phoneViewController.SelectedCard.Card.CompanyName : phoneViewController.SelectedCard.Card.Name;
			}

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_PHONE, name, 0);
		}

		void ShowMaps ()
		{

			if (SelectedCard.Card.Addresses != null && SelectedCard.Card.Addresses.Any ()) {
				string address = buildAddress ();
				var url = new NSUrl ("http://www.maps.apple.com/?daddr=" + System.Net.WebUtility.UrlEncode (address.Trim ()));

				if (UIApplication.SharedApplication.CanOpenUrl (url)) {
					UIApplication.SharedApplication.OpenUrl (url);	
				} else {
					new UIAlertView ("Error", "Maps is not supported on this device", null, "Ok").Show ();
				}
					

				ActivityController.SaveActivity ((long)EventSources.Map, SelectedCard.Card.CardId, UISubscriptionService.AuthToken);

				string name = Resources.GA_LABEL_MAP;
				if (SelectedCard != null && SelectedCard.Card != null) {
					name = string.IsNullOrEmpty (SelectedCard.Card.Name) ? SelectedCard.Card.CompanyName : SelectedCard.Card.Name;
				}

				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_MAP, name, 0);
			}
		}

		string buildAddress ()
		{

			var address = string.Empty;
			var card = SelectedCard.Card;

			address += string.IsNullOrEmpty (SelectedCard.Card.Addresses [0].Address1) ? string.Empty : card.Addresses [0].Address1;
			address += " ";
			address += string.IsNullOrEmpty (card.Addresses [0].Address2) ? string.Empty : card.Addresses [0].Address2;
			address += " ";
			address += string.IsNullOrEmpty (card.Addresses [0].City) ? string.Empty : card.Addresses [0].City;
			address += " ";
			address += card.Addresses [0].State == null ? string.Empty : card.Addresses [0].State.Code;
			address += " ";
			address += string.IsNullOrEmpty (card.Addresses [0].ZipCode) ? string.Empty : card.Addresses [0].ZipCode;

			return address;
		}
	}
}
