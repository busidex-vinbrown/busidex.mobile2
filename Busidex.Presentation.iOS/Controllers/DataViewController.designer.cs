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
	[Register ("DataViewController")]
	partial class DataViewController
	{
		[Outlet]
		UIKit.UILabel dataLabel { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView spnWait { get; set; }

		[Outlet]
		UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		UIKit.UITextField txtUserName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnEvents { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGoToMyBusidex { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGoToSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnMyOrganizations { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnQuestions { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblEvents { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblMyBusidex { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblOrganizations { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblProgress { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblQuestions { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblSearch { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnEvents != null) {
				btnEvents.Dispose ();
				btnEvents = null;
			}
			if (btnGoToMyBusidex != null) {
				btnGoToMyBusidex.Dispose ();
				btnGoToMyBusidex = null;
			}
			if (btnGoToSearch != null) {
				btnGoToSearch.Dispose ();
				btnGoToSearch = null;
			}
			if (btnMyOrganizations != null) {
				btnMyOrganizations.Dispose ();
				btnMyOrganizations = null;
			}
			if (btnQuestions != null) {
				btnQuestions.Dispose ();
				btnQuestions = null;
			}
			if (lblEvents != null) {
				lblEvents.Dispose ();
				lblEvents = null;
			}
			if (lblMyBusidex != null) {
				lblMyBusidex.Dispose ();
				lblMyBusidex = null;
			}
			if (lblOrganizations != null) {
				lblOrganizations.Dispose ();
				lblOrganizations = null;
			}
			if (lblProgress != null) {
				lblProgress.Dispose ();
				lblProgress = null;
			}
			if (lblQuestions != null) {
				lblQuestions.Dispose ();
				lblQuestions = null;
			}
			if (lblSearch != null) {
				lblSearch.Dispose ();
				lblSearch = null;
			}
		}
	}
}
