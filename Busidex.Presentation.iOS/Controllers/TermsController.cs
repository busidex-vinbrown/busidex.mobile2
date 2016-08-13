
using System;
using Foundation;
using GoogleAnalytics.iOS;
using System.Linq;

namespace Busidex.Presentation.iOS
{
	public partial class TermsController : BaseController
	{
		public TermsController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
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
			vwTerms.LoadRequest (new NSUrlRequest (new NSUrl (Mobile.Resources.TERMS_AND_CONDITIONS_URL)));

			btnPrivacy.TouchUpInside += delegate {
				showPrivacy ();
			};
		}

		void showPrivacy ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is PrivacyController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.privacyController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.privacyController, true);
			}
		}
	}
}

