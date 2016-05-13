using System;
using Foundation;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class PrivacyController : BaseController
	{
		public PrivacyController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "View Terms");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			vwPrivacy.LoadRequest (new NSUrlRequest (new NSUrl (Busidex.Mobile.Resources.PRIVACY_URL)));

			btnTerms.TouchUpInside += delegate {
				GoToTerms ();
			};
		}
	}
}


