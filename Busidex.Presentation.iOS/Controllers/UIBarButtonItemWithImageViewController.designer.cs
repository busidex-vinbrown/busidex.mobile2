// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Busidex.Presentation.iOS
{
	[Register ("UIBarButtonItemWithImageViewController")]
	partial class UIBarButtonItemWithImageViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwipeGestureRecognizer SwiperLeft { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwipeGestureRecognizer SwiperRight { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (SwiperLeft != null) {
				SwiperLeft.Dispose ();
				SwiperLeft = null;
			}
			if (SwiperRight != null) {
				SwiperRight.Dispose ();
				SwiperRight = null;
			}
		}
	}
}
