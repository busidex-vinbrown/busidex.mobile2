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
    [Register ("QuickShareController")]
    partial class QuickShareController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgSharedCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPersonalMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgSharedCard != null) {
                imgSharedCard.Dispose ();
                imgSharedCard = null;
            }

            if (lblMessage != null) {
                lblMessage.Dispose ();
                lblMessage = null;
            }

            if (lblPersonalMessage != null) {
                lblPersonalMessage.Dispose ();
                lblPersonalMessage = null;
            }
        }
    }
}