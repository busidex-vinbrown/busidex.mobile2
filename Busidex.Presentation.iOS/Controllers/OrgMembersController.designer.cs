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
	[Register ("OrgMembersController")]
	partial class OrgMembersController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgLogo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tblMembers { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISearchBar txtSearch { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (imgLogo != null) {
				imgLogo.Dispose ();
				imgLogo = null;
			}
			if (tblMembers != null) {
				tblMembers.Dispose ();
				tblMembers = null;
			}
			if (txtSearch != null) {
				txtSearch.Dispose ();
				txtSearch = null;
			}
		}
	}
}
