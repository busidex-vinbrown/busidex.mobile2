﻿// WARNING
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
    [Register ("SharedCardController")]
    partial class SharedCardController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnContacts { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblError { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSuccess { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ShareCardView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtDisplayName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        PlaceholderEnabledUITextView.PlaceholderEnabledUITextView txtPersonalMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPhoneNumber { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnContacts != null) {
                btnContacts.Dispose ();
                btnContacts = null;
            }

            if (imgCard != null) {
                imgCard.Dispose ();
                imgCard = null;
            }

            if (lblError != null) {
                lblError.Dispose ();
                lblError = null;
            }

            if (lblSuccess != null) {
                lblSuccess.Dispose ();
                lblSuccess = null;
            }

            if (ShareCardView != null) {
                ShareCardView.Dispose ();
                ShareCardView = null;
            }

            if (txtDisplayName != null) {
                txtDisplayName.Dispose ();
                txtDisplayName = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtPersonalMessage != null) {
                txtPersonalMessage.Dispose ();
                txtPersonalMessage = null;
            }

            if (txtPhoneNumber != null) {
                txtPhoneNumber.Dispose ();
                txtPhoneNumber = null;
            }
        }
    }
}