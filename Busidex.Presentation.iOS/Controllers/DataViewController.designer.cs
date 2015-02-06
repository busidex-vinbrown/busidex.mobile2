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
		UIButton btnGoToMyBusidex { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGoToSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnMyOrganizations { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblProgress { get; set; }

		void ReleaseDesignerOutlets ()
		{
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
			if (lblProgress != null) {
				lblProgress.Dispose ();
				lblProgress = null;
			}
		}
	}
}
