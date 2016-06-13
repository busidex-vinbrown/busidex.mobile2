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
    [Register ("OrganizationDetailController")]
    partial class OrganizationDetailController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBrowser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnFacebook { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnMembers { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnReferrals { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTwitter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgOrgImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblContacts { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtFax { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtPhone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView wvMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnBrowser != null) {
                btnBrowser.Dispose ();
                btnBrowser = null;
            }

            if (btnFacebook != null) {
                btnFacebook.Dispose ();
                btnFacebook = null;
            }

            if (btnMembers != null) {
                btnMembers.Dispose ();
                btnMembers = null;
            }

            if (btnReferrals != null) {
                btnReferrals.Dispose ();
                btnReferrals = null;
            }

            if (btnTwitter != null) {
                btnTwitter.Dispose ();
                btnTwitter = null;
            }

            if (imgOrgImage != null) {
                imgOrgImage.Dispose ();
                imgOrgImage = null;
            }

            if (lblContacts != null) {
                lblContacts.Dispose ();
                lblContacts = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtFax != null) {
                txtFax.Dispose ();
                txtFax = null;
            }

            if (txtPhone != null) {
                txtPhone.Dispose ();
                txtPhone = null;
            }

            if (wvMessage != null) {
                wvMessage.Dispose ();
                wvMessage = null;
            }
        }
    }
}