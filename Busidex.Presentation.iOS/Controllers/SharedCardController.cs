
using System;
using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;
using GoogleAnalytics.iOS;
using CoreAnimation;
using CoreGraphics;
using Plugin.Messaging;

namespace Busidex.Presentation.iOS
{
	public partial class SharedCardController : BaseController
	{
		public UserCard UserCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }

		public SharedCardController (IntPtr handle) : base (handle)
		{
		}

		void LoadCard(){

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {

				var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
				UserCard userCard = null;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						var myBusidexJson = myBusidexFile.ReadToEnd ();
						MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
						foreach(var uc in myBusidexResponse.MyBusidex.Busidex){
							if(uc.Card.CardId == UserCard.Card.CardId){
								userCard = uc;
								break;
							}
						}
					}
				}

				if (userCard != null) {
					FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + userCard.Card.FrontFileName);
					if (File.Exists (FrontFileName)) {
						imgCard.Image = UIImage.FromFile (FrontFileName);
						imgCard.Layer.AddSublayer (GetBorder (imgCard.Frame, UIColor.Gray.CGColor));
						//imgCard.Layer.AddSublayer (GetBorder (imgCard.Frame, UIColor.LightGray.CGColor, 1f, 1.5f));
						//imgCard.Layer.AddSublayer (GetBorder (imgCard.Frame, UIColor.LightGray.CGColor, 2f));
					}
				}
			}
			imgCardShared.Hidden = true;
		}

		void UpdateDisplayName(string token){

			var displayName = txtDisplayName.Text;
			var user = NSUserDefaults.StandardUserDefaults;
			var savedDisplayName = user.StringForKey (Resources.USER_SETTING_DISPLAYNAME);

			if(!displayName.Equals(savedDisplayName)){
				AccountController.UpdateDisplayName (displayName, token);
				user.SetString (displayName, Resources.USER_SETTING_DISPLAYNAME);
				user.Synchronize ();
			}
		}

		public void ShareCard(){

			if(string.IsNullOrEmpty(txtDisplayName.Text)){
				ShowAlert ("Missing Information", "Please enter your display name", "Ok");
				txtDisplayName.BecomeFirstResponder ();
				return;
			}

			string phoneNumber = txtPhoneNumber.Text;
			if(!string.IsNullOrEmpty(phoneNumber)){
				phoneNumber = phoneNumber.Replace ("(", "").Replace (")", "").Replace (".", "").Replace ("-", "").Replace(" ", "");
			}
			string email = txtEmail.Text;

			if(string.IsNullOrEmpty(phoneNumber) && (string.IsNullOrEmpty(email) || email.IndexOf("@") < 0)){
				ShowAlert ("Missing Information", "Please enter an email address or phone number", "Ok");
				txtEmail.BecomeFirstResponder ();
				return;
			}

			var cookie = GetAuthCookie ();
			if(cookie == null){
				return;
			}

			UpdateDisplayName (cookie.Value);

			lblError.Hidden = true;

			var controller = new Busidex.Mobile.SharedCardController ();

			string response;
			if(string.IsNullOrEmpty(phoneNumber)){
				// send the shared card the 'traditional' way
				response = controller.ShareCard (UserCard.Card, email, phoneNumber, cookie.Value);
				if( !string.IsNullOrEmpty(response) && response.Contains("true")){
					imgCardShared.Hidden = false;
				}else{
					lblError.Hidden = false;
					imgCardShared.Hidden = true;
				}
			}else{
				// send text message with quick share link
				var smsTask = MessagingPlugin.SmsMessenger;
				if (smsTask.CanSendSms) {
					EmailTemplateController.GetTemplate (EmailTemplateCode.SharedCardSMS, cookie.Value).ContinueWith (r => {

						var user = NSUserDefaults.StandardUserDefaults;
						var displayName = user.StringForKey(Resources.USER_SETTING_DISPLAYNAME);

						var template = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplateResponse> (r.Result);
						if(template != null){
							string message = string.Format(template.Template.Subject, displayName) + Environment.NewLine + Environment.NewLine + 
								string.Format(template.Template.Body, UserCard.Card.CardId, Utils.DecodeUserId(cookie.Value), displayName, Environment.NewLine);
						
							InvokeOnMainThread (() => smsTask.SendSms ("+" + phoneNumber, message));
						}
					});
				}
				imgCardShared.Hidden = false;
			}
		}

		CALayer GetBorder(CGRect frame, CGColor color, float offset = 0f, float borderWidth = 1f ){
			var layer = new CALayer ();
			layer.Bounds = new CGRect (frame.X, frame.Y, frame.Width + offset, frame.Height + offset);
			layer.Position = new CGPoint ((frame.Width / 2f) + offset, (frame.Height / 2f) + offset);
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = borderWidth;
			layer.BorderColor = color;

			return layer;
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Share Card");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 100f, 25f);
				var shareImage = UIButton.FromType (UIButtonType.System);
				shareImage.Frame = imgFrame;
				shareImage.Font = UIFont.FromName ("Helvetica", 17f);

				shareImage.SetTitle ("Share", UIControlState.Normal);
				shareImage.TouchUpInside += ((s, e) => ShareCard ());
				var shareButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
				shareButton.CustomView = shareImage;

				NavigationItem.SetRightBarButtonItem(
					shareButton, true);
			}

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			lblError.Hidden = true;
			imgCardShared.Hidden = true;

			var user = NSUserDefaults.StandardUserDefaults;
			var displayName = user.StringForKey (Resources.USER_SETTING_DISPLAYNAME);
			if(string.IsNullOrEmpty(displayName)){
				var token = GetAuthCookie ().Value;
				var accountResponse = AccountController.GetAccount (token);
				var account = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountResponse);
				displayName = account.UserAccount.DisplayName;
			}
			txtDisplayName.Text = displayName;

			LoadCard ();

		}
	}
}

