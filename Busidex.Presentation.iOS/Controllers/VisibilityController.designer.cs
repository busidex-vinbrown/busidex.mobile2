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
    [Register ("VisibilityController")]
    partial class VisibilityController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPrivate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPublic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSemiPublic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPrivate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPublic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSemiPublic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtDescription { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPrivate != null) {
                btnPrivate.Dispose ();
                btnPrivate = null;
            }

            if (btnPublic != null) {
                btnPublic.Dispose ();
                btnPublic = null;
            }

            if (btnSemiPublic != null) {
                btnSemiPublic.Dispose ();
                btnSemiPublic = null;
            }

            if (lblPrivate != null) {
                lblPrivate.Dispose ();
                lblPrivate = null;
            }

            if (lblPublic != null) {
                lblPublic.Dispose ();
                lblPublic = null;
            }

            if (lblSemiPublic != null) {
                lblSemiPublic.Dispose ();
                lblSemiPublic = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (txtDescription != null) {
                txtDescription.Dispose ();
                txtDescription = null;
            }
        }
    }
}