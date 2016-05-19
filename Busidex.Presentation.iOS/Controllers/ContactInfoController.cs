using System;

using UIKit;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class ContactInfoController : BaseCardEditController
	{
		public ContactInfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Contact Info");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


		}
			
	}
}


