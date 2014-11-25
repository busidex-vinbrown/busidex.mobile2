// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace Busidex.Presentation.iOS
{
	[Register ("OrganizationDetailController")]
	partial class OrganizationDetailController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnBrowser { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnFacebook { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnTwitter { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgOrgImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblContacts { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView txtEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView txtFax { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView txtPhone { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView wvMessage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnBrowser != null) {
				btnBrowser.Dispose ();
				btnBrowser = null;
			}
			if (btnFacebook != null) {
				btnFacebook.Dispose ();
				btnFacebook = null;
			}
			if (btnTwitter != null) {
				btnTwitter.Dispose ();
				btnTwitter = null;
			}
			if (imgOrgImage != null) {
				imgOrgImage.Dispose ();
				imgOrgImage = null;
			}
			if (lblContacts != null) {
				lblContacts.Dispose ();
				lblContacts = null;
			}
			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}
			if (txtFax != null) {
				txtFax.Dispose ();
				txtFax = null;
			}
			if (txtPhone != null) {
				txtPhone.Dispose ();
				txtPhone = null;
			}
			if (wvMessage != null) {
				wvMessage.Dispose ();
				wvMessage = null;
			}
		}
	}
}
