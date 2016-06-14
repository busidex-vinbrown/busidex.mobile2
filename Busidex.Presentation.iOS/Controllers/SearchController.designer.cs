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
    [Register ("SearchController")]
    partial class SearchController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgBackground { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar txtSearch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView vwSearchResults { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgBackground != null) {
                imgBackground.Dispose ();
                imgBackground = null;
            }

            if (txtSearch != null) {
                txtSearch.Dispose ();
                txtSearch = null;
            }

            if (vwSearchResults != null) {
                vwSearchResults.Dispose ();
                vwSearchResults = null;
            }
        }
    }
}