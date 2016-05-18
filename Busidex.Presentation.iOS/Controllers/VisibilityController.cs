using System;

using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public partial class VisibilityController : BaseCardEditController
	{
		

		public VisibilityController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Visibility");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


