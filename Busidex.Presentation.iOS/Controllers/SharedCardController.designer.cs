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
	[Register ("SharedCardController")]
	partial class SharedCardController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgCard { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgCardShared { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblError { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ShareCardView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtEmail { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (imgCard != null) {
				imgCard.Dispose ();
				imgCard = null;
			}
			if (imgCardShared != null) {
				imgCardShared.Dispose ();
				imgCardShared = null;
			}
			if (lblError != null) {
				lblError.Dispose ();
				lblError = null;
			}
			if (ShareCardView != null) {
				ShareCardView.Dispose ();
				ShareCardView = null;
			}
			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}
		}
	}
}
