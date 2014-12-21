using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Linq;
using System.Drawing;
using System.IO;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	partial class PhoneViewController : BaseController
	{
		public UserCard UserCard{ get; set; }
		string userToken;
		bool UseStar82;

		public PhoneViewController (IntPtr handle) : base (handle)
		{

		}

		void LoadCard(){
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {
				var FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + UserCard.Card.FrontFileName);
				if (File.Exists (FrontFileName)) {

					imgCard.Image = UIImage.FromFile (FrontFileName);

				}
			}

			var labelX = 25f;
			var labelY = 280f;
			var labelHeight = 30f;
			var labelWidth = 100f;

			var labelFrame = new RectangleF (labelX, labelY, labelWidth, labelHeight);
			var phoneFrame = new RectangleF (labelX + labelWidth + 10, labelY, labelWidth * 2, labelHeight);

			if (UserCard != null && UserCard.Card != null && UserCard.Card.PhoneNumbers != null) {

				foreach (PhoneNumber number in UserCard.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))) {

					var newLabel = new UILabel (labelFrame);
					var newNumber = UIButton.FromType (UIButtonType.System); //new UITextView (phoneFrame);
					if (number.PhoneNumberType != null) {
						newLabel.Text = Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId);

						newLabel.Font = UIFont.FromName ("Helvetica", 20f);
						newLabel.UserInteractionEnabled = true;
						newLabel.TextColor = UIColor.FromRGB(66,69,76);

						var textAttributed = new NSMutableAttributedString (
							number.Number, 
							new UIStringAttributes  {
								ForegroundColor = UIColor.Blue, 
								Font = UIFont.FromName ("Helvetica", 22f),
								UnderlineStyle = NSUnderlineStyle.Single 
							}
						);
						newNumber.SetAttributedTitle(textAttributed, UIControlState.Normal);
						newNumber.UserInteractionEnabled = true;
						newNumber.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
						newNumber.UserInteractionEnabled = true;
						newNumber.Frame = phoneFrame;
						newNumber.TouchUpInside += delegate {
							UIApplication.SharedApplication.OpenUrl (new NSUrl ("tel:" + (UseStar82 ? "*82." : string.Empty) + number.Number));
							//NewRelic.NewRelic.RecordMetricWithName (UIMetrics.WEBSITE_VISIT, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
							ActivityController.SaveActivity ((long)EventSources.Call, UserCard.Card.CardId, userToken);
						};
						labelFrame.Y = phoneFrame.Y += 35;

						View.Add (newLabel);
						View.Add (newNumber);
					}
				}
			}
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

				var user = NSUserDefaults.StandardUserDefaults;
				UseStar82 = user.BoolForKey(Resources.USER_SETTING_USE_STAR_82);

			}catch(Exception ex){
				LoggingController.LogError (ex, userToken);
			}
		}
	}
}
