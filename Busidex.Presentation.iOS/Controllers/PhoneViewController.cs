using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Linq;
using System.Drawing;
using System.IO;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class PhoneViewController : BaseController
	{
		public UserCard SelectedCard{ get; set; }
		string userToken;

		public PhoneViewController (IntPtr handle) : base (handle)
		{

		}

		void LoadCard(){
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (SelectedCard != null && SelectedCard.Card != null) {

				BusinessCardDimensions dimensions = GetCardDimensions (SelectedCard.Card.FrontOrientation);
				imgCard.Frame = new CoreGraphics.CGRect (dimensions.MarginLeft, 75f, dimensions.Width, dimensions.Height);

				var FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
				if (File.Exists (FrontFileName)) {
					imgCard.Image = UIImage.FromFile (FrontFileName);
				}else{
					ShowOverlay ();
					Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.Card.FrontFileName, documentsPath, SelectedCard.Card.FrontFileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							if(SelectedCard.Card.BackOrientation == "H"){
								imgCard.Image = new UIImage(UIImage.FromFile (FrontFileName).CGImage, 1, UIImageOrientation.Right);
							}else{
								imgCard.Image = UIImage.FromFile (FrontFileName);
							}
							Overlay.Hide();
						});
					});
				}
			}

			const float labelX = 10f;
			const float labelY = 280f;
			const float labelHeight = 40f;
			const float labelWidth = 80f;
			const float phoneFrameWidth = 110f;
			const float imageWidth = 40f;
			const float imageHeight = 40f;
			const float phoneFrameX = labelX + labelWidth + 10;
			const float phoneImageX = phoneFrameX + phoneFrameWidth + 10;
			const float textImageX = phoneImageX + imageWidth + 10;

			var labelFrame = new RectangleF (labelX, labelY, labelWidth, labelHeight);
			var phoneFrame = new RectangleF (phoneFrameX, labelY, phoneFrameWidth, labelHeight);
			var phoneImageFrame = new RectangleF (phoneImageX, labelY, imageWidth, imageHeight);
			var textImageFrame = new RectangleF (textImageX, labelY, imageWidth, imageHeight);

			if (SelectedCard != null && SelectedCard.Card != null && SelectedCard.Card.PhoneNumbers != null) {

				foreach (PhoneNumber number in SelectedCard.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))) {

					var newLabel = new UILabel (labelFrame);
					var newNumber = new UILabel (phoneFrame);
					var newPhoneImage = UIButton.FromType (UIButtonType.Custom);
					var newTextImage = UIButton.FromType (UIButtonType.Custom);

					if (number.PhoneNumberType != null) {
						newLabel.Text = Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId);

						newLabel.Font = UIFont.FromName ("Helvetica", 18f);
						newLabel.UserInteractionEnabled = true;
						newLabel.TextColor = UIColor.FromRGB(66,69,76);
						newLabel.TextAlignment = UITextAlignment.Right;

//						var textAttributed = new NSMutableAttributedString (
//							number.Number, 
//							new UIStringAttributes  {
//								ForegroundColor = UIColor.Blue, 
//								Font = UIFont.FromName ("Helvetica", 16f),
//								UnderlineStyle = NSUnderlineStyle.Single 
//							}
//						);
						newNumber.Text = number.Number;
						newNumber.Font = UIFont.FromName ("Helvetica", 16f);
						newNumber.TextColor = UIColor.Black;
						newNumber.TextAlignment = UITextAlignment.Left;

						newPhoneImage.Frame = phoneImageFrame;
						newPhoneImage.SetImage (UIImage.FromFile ("phone.png"), UIControlState.Normal);
						newPhoneImage.TouchUpInside += delegate {

							string pn = number.Number.Replace("(", "").Replace(")", "").Replace("-","").Replace(" ", "");
							var phoneNumber = new NSUrl ("telprompt://" + pn);

							if(!UIApplication.SharedApplication.OpenUrl (phoneNumber)){
								var av = new UIAlertView ("Phone Number Error",
									"We are unable to dial the number: " + number.Number + ". You may need to dial this number manually.",
									null,
									"OK",
									null);
								av.Show ();
							}else{
								//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.WEBSITE_VISIT, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
								ActivityController.SaveActivity ((long)EventSources.Call, SelectedCard.Card.CardId, userToken);
							}
						};

						newTextImage.Frame = textImageFrame;
						newTextImage.SetImage (UIImage.FromFile ("textmessage.png"), UIControlState.Normal);
						newTextImage.TouchUpInside += delegate {

							string pn = number.Number.Replace("(", "").Replace(")", "").Replace("-","").Replace(" ", "");
							var phoneNumber = new NSUrl ("sms:" + pn);

							if(!UIApplication.SharedApplication.OpenUrl (phoneNumber)){
								var av = new UIAlertView ("Phone Number Error",
									"We are unable to dial the number: " + number.Number + ". You may need to dial this number manually.",
									null,
									"OK",
									null);
								av.Show ();
							}else{
								ActivityController.SaveActivity ((long)EventSources.Call, SelectedCard.Card.CardId, userToken);
							}
						};

						labelFrame.Y = phoneFrame.Y = phoneImageFrame.Y = textImageFrame.Y += 55;

						View.Add (newLabel);
						View.Add (newNumber);
						View.Add (newPhoneImage);
						View.Add (newTextImage);
					}
				}
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Phone");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			try{
				documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				LoadCard ();

				var cookie = GetAuthCookie ();

				if (cookie != null) {
					userToken = cookie.Value;
				}

			}catch(Exception ex){
				LoggingController.LogError (ex, userToken);
			}
		}
	}
}
