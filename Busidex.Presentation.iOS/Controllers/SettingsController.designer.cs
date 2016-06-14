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
    [Register ("SettingsController")]
    partial class SettingsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnMyCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgEmailSaved { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEmailError { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView lblInstructions { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView padding { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnMyCard != null) {
                btnMyCard.Dispose ();
                btnMyCard = null;
            }

            if (btnTerms != null) {
                btnTerms.Dispose ();
                btnTerms = null;
            }

            if (imgEmailSaved != null) {
                imgEmailSaved.Dispose ();
                imgEmailSaved = null;
            }

            if (lblEmailError != null) {
                lblEmailError.Dispose ();
                lblEmailError = null;
            }

            if (lblInstructions != null) {
                lblInstructions.Dispose ();
                lblInstructions = null;
            }

            if (padding != null) {
                padding.Dispose ();
                padding = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }
        }
    }
}