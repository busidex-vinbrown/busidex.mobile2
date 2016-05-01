﻿
using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;
using GoogleAnalytics.iOS;
using CoreGraphics;
using Plugin.Messaging;
using System.Net;
using ContactsUI;
using Contacts;

namespace Busidex.Presentation.iOS
{
	public partial class SharedCardController : BaseController
	{
		public UserCard SelectedCard{ get; set; }

		string FrontFileName{ get; set; }

		string BackFileName{ get; set; }

		public SharedCardController (IntPtr handle) : base (handle)
		{
		}

		void LoadCard ()
		{

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (SelectedCard != null && SelectedCard.Card != null) {

				BusinessCardDimensions dimensions = GetCardDimensions (SelectedCard.Card.FrontOrientation);
				imgCard.Frame = new CGRect (dimensions.MarginLeft, 340f, dimensions.Width, dimensions.Height);

				FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				if (File.Exists (FrontFileName)) {
					imgCard.Image = UIImage.FromFile (FrontFileName);
					imgCard.Layer.AddSublayer (GetBorder (imgCard.Frame, UIColor.Gray.CGColor));
				} else {
					ShowOverlay ();
					Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.Card.FrontFileName, documentsPath, SelectedCard.Card.FrontFileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							imgCard.Image = SelectedCard.Card.BackOrientation == "H" 
								? new UIImage (UIImage.FromFile (FrontFileName).CGImage, 1, UIImageOrientation.Right) 
								: UIImage.FromFile (FrontFileName);
							Overlay.Hide ();
						});
					});
				}

			}
			imgCardShared.Hidden = true;
		}

		void UpdateDisplayName (string token)
		{

			var displayName = txtDisplayName.Text;
			var user = NSUserDefaults.StandardUserDefaults;
			var savedDisplayName = user.StringForKey (Resources.USER_SETTING_DISPLAYNAME);

			if (!displayName.Equals (savedDisplayName)) {
				AccountController.UpdateDisplayName (displayName, token);
				user.SetString (displayName, Resources.USER_SETTING_DISPLAYNAME);
				user.Synchronize ();
			}
		}

		public void ShareCard ()
		{

			if (string.IsNullOrEmpty (txtDisplayName.Text)) {
				ShowAlert ("Missing Information", "Please enter your display name", "Ok");
				txtDisplayName.BecomeFirstResponder ();
				return;
			}

			string phoneNumber = txtPhoneNumber.Text;
			if (!string.IsNullOrEmpty (phoneNumber)) {
				phoneNumber = phoneNumber.Replace ("(", "").Replace (")", "").Replace (".", "").Replace ("-", "").Replace (" ", "");
			}
			string email = txtEmail.Text;
			var personalMessage = txtPersonalMessage.Text.Replace ("\n", "  ");

			if (string.IsNullOrEmpty (phoneNumber) && (string.IsNullOrEmpty (email) || email.IndexOf ("@", StringComparison.Ordinal) < 0)) {
				ShowAlert ("Missing Information", "Please enter an email address or phone number", "Ok");
				txtEmail.BecomeFirstResponder ();
				return;
			}

			var cookie = GetAuthCookie ();
			if (cookie == null) {
				return;
			}

			UpdateDisplayName (cookie.Value);

			lblError.Hidden = true;

			var controller = new Busidex.Mobile.SharedCardController ();

			string response;
			if (string.IsNullOrEmpty (phoneNumber)) {
				// send the shared card the 'traditional' way
				response = controller.ShareCard (SelectedCard.Card, email, phoneNumber, cookie.Value);
				if (!string.IsNullOrEmpty (response) && response.Contains ("true")) {
					imgCardShared.Hidden = false;
				} else {
					lblError.Hidden = false;
					imgCardShared.Hidden = true;
				}
			} else {
				// send text message with quick share link
				var smsTask = MessagingPlugin.SmsMessenger;
				if (smsTask.CanSendSms || true) {
					EmailTemplateController.GetTemplate (EmailTemplateCode.SharedCardSMS, cookie.Value).ContinueWith (r => {

						var user = NSUserDefaults.StandardUserDefaults;
						var displayName = user.StringForKey (Resources.USER_SETTING_DISPLAYNAME);

						var template = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplateResponse> (r.Result);
						if (template != null) {
							string message = string.Format (template.Template.Subject, displayName) + Environment.NewLine + Environment.NewLine +
							                 string.Format (template.Template.Body, SelectedCard.Card.CardId, Utils.DecodeUserId (cookie.Value), displayName, personalMessage,
								                 Environment.NewLine);
						
							int startIdx = message.IndexOf ('[');
							int endIdx = message.IndexOf (']');
							string originalUrl = message.Substring (startIdx + 1, endIdx - startIdx - 2);
							string url = WebUtility.UrlEncode (originalUrl);

							UrlShortenerController.ShortenUrl (url).ContinueWith (resp => {

								var bitlyResponse = resp.Result;
								if (bitlyResponse != null) {
									message = message.Replace (originalUrl, bitlyResponse).Replace ("[", "").Replace ("]", "");

									InvokeOnMainThread (() => {
										smsTask.SendSms (phoneNumber, message);
										resetFields ();
									});
								}
							});


						}
					});
				}
				imgCardShared.Hidden = false;
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Share Card");

			base.ViewDidAppear (animated);
		}

		void resetFields ()
		{
			txtPersonalMessage.Text = string.Empty;
			txtPhoneNumber.Text = string.Empty;
			txtEmail.Text = string.Empty;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			lblError.Hidden = true;
			imgCardShared.Hidden = true;

			var user = NSUserDefaults.StandardUserDefaults;
			var displayName = user.StringForKey (Resources.USER_SETTING_DISPLAYNAME);
			if (string.IsNullOrEmpty (displayName)) {
				var token = GetAuthCookie ().Value;
				var accountResponse = AccountController.GetAccount (token);
				var account = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountResponse);
				displayName = account.UserAccount.DisplayName;
			}
			txtDisplayName.Text = displayName;

			LoadCard ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UITextFieldCondition returnCallback = textField => {
				textField.ResignFirstResponder ();
				return true;
			};
			UITextViewCondition tvReturnCallback = textField => {
				if (textField == null)
					throw new ArgumentNullException ("textField");
				textField.ResignFirstResponder ();
				return true;
			};
			EventHandler editingCallback = (textField, e) => ((UITextField)textField).ResignFirstResponder ();
			txtEmail.ShouldReturn += returnCallback;
			txtDisplayName.ShouldReturn += returnCallback;
			txtPhoneNumber.ShouldReturn += returnCallback;

			txtPhoneNumber.EditingDidEnd += editingCallback;
			txtPersonalMessage.ShouldEndEditing += tvReturnCallback;
			txtEmail.EditingDidEnd += editingCallback;
			txtDisplayName.EditingDidEnd += editingCallback;

			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				var imgFrame = new CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 100f, 25f);
				var shareImage = UIButton.FromType (UIButtonType.System);
				shareImage.Frame = imgFrame;
				shareImage.Font = UIFont.FromName ("Helvetica", 17f);

				shareImage.SetTitle ("Share", UIControlState.Normal);
				shareImage.TouchUpInside += ((s, e) => ShareCard ());
				var shareButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
				shareButton.CustomView = shareImage;

				NavigationItem.SetRightBarButtonItem (
					shareButton, true);

				// Create a new picker
				try {
					var picker = new CNContactPickerViewController ();

					// Select property to pick
					picker.DisplayedPropertyKeys = new [] { CNContactKey.PhoneNumbers };
					picker.PredicateForEnablingContact = NSPredicate.FromFormat ("phoneNumbers.@count > 0");
					picker.PredicateForSelectionOfContact = NSPredicate.FromFormat ("phoneNumbers.@count == 0"); // always allow the user to see the contact details


					// Respond to selection
					picker.Delegate = new ContactPickerDelegate (txtPhoneNumber);

					// Display picker
					btnContacts.TouchUpInside += delegate {
						try {
							PresentModalViewController (picker, true);
						} catch (Exception ex) {
							Xamarin.Insights.Report (ex);
							ShowAlert ("Upgrade Required", "This feature requires iOS 9 or higher.", "Ok");
						}
					};

				} catch (Exception ex) {
					Xamarin.Insights.Report (ex);
					ShowAlert ("Upgrade Required", "This feature requires iOS 9 or higher.", "Ok");
				}
			}
		}
	}
}

