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
	[Register ("OrganizationsController")]
	partial class OrganizationsController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISearchBar txtSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView vwOrganizations { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (txtSearch != null) {
				txtSearch.Dispose ();
				txtSearch = null;
			}
			if (vwOrganizations != null) {
				vwOrganizations.Dispose ();
				vwOrganizations = null;
			}
		}
	}
}
