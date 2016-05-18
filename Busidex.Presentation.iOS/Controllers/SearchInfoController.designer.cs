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
	[Register ("SearchInfoController")]
	partial class SearchInfoController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView lblInstructions { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtCompanyName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TPKeyboardAvoiding.TPKeyboardAvoidingScrollView vwFields { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (lblInstructions != null) {
				lblInstructions.Dispose ();
				lblInstructions = null;
			}
			if (txtCompanyName != null) {
				txtCompanyName.Dispose ();
				txtCompanyName = null;
			}
			if (txtName != null) {
				txtName.Dispose ();
				txtName = null;
			}
			if (txtTitle != null) {
				txtTitle.Dispose ();
				txtTitle = null;
			}
			if (vwFields != null) {
				vwFields.Dispose ();
				vwFields = null;
			}
		}
	}
}
