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
        UIKit.UILabel lblDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtCompanyName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView vwPage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
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

            if (vwPage != null) {
                vwPage.Dispose ();
                vwPage = null;
            }
        }
    }
}