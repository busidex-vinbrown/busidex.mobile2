// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Busidex.Presentation.iOS.Controllers
{
    [Register ("EventCardsController")]
    partial class EventCardsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEventName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblEventCards { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar txtFilter { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblEventName != null) {
                lblEventName.Dispose ();
                lblEventName = null;
            }

            if (tblEventCards != null) {
                tblEventCards.Dispose ();
                tblEventCards = null;
            }

            if (txtFilter != null) {
                txtFilter.Dispose ();
                txtFilter = null;
            }
        }
    }
}