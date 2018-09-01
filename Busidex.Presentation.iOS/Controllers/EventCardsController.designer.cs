// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;

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