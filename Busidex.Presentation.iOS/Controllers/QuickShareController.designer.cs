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
	[Register ("QuickShareController")]
	partial class QuickShareController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgSharedCard { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblMessage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblPersonalMessage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (imgSharedCard != null) {
				imgSharedCard.Dispose ();
				imgSharedCard = null;
			}
			if (lblMessage != null) {
				lblMessage.Dispose ();
				lblMessage = null;
			}
			if (lblPersonalMessage != null) {
				lblPersonalMessage.Dispose ();
				lblPersonalMessage = null;
			}
		}
	}
}
