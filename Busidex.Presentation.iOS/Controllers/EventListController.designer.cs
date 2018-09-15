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
    [Register ("EventListController")]
    partial class EventListController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView vwEventList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (vwEventList != null) {
                vwEventList.Dispose ();
                vwEventList = null;
            }
        }
    }
}