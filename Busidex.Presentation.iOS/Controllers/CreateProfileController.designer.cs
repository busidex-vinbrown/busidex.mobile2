﻿// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Busidex.Presentation.iOS
{
    [Register ("CreateProfileController")]
    partial class CreateProfileController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAcceptTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgAccept { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgEmailSaved { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgPasswordSaved { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEmailError { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView lblInstructions { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPasswordError { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView padding { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPassword { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAcceptTerms != null) {
                btnAcceptTerms.Dispose ();
                btnAcceptTerms = null;
            }

            if (btnTerms != null) {
                btnTerms.Dispose ();
                btnTerms = null;
            }

            if (imgAccept != null) {
                imgAccept.Dispose ();
                imgAccept = null;
            }

            if (imgEmailSaved != null) {
                imgEmailSaved.Dispose ();
                imgEmailSaved = null;
            }

            if (imgPasswordSaved != null) {
                imgPasswordSaved.Dispose ();
                imgPasswordSaved = null;
            }

            if (lblEmailError != null) {
                lblEmailError.Dispose ();
                lblEmailError = null;
            }

            if (lblInstructions != null) {
                lblInstructions.Dispose ();
                lblInstructions = null;
            }

            if (lblPassword != null) {
                lblPassword.Dispose ();
                lblPassword = null;
            }

            if (lblPasswordError != null) {
                lblPasswordError.Dispose ();
                lblPasswordError = null;
            }

            if (padding != null) {
                padding.Dispose ();
                padding = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }
        }
    }
}