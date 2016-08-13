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
    [Register ("HomeController")]
    partial class HomeController
    {
        [Outlet]
        UIKit.UILabel dataLabel { get; set; }


        [Outlet]
        UIKit.UIActivityIndicatorView spnWait { get; set; }


        [Outlet]
        UIKit.UITextField txtPassword { get; set; }


        [Outlet]
        UIKit.UITextField txtUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnEvents { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnGoToMyBusidex { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnGoToSearch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnMyOrganizations { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnShare { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblEvents { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMyBusidex { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblOrganizations { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblProgress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblQuestions { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSearch { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnEvents != null) {
                btnEvents.Dispose ();
                btnEvents = null;
            }

            if (btnGoToMyBusidex != null) {
                btnGoToMyBusidex.Dispose ();
                btnGoToMyBusidex = null;
            }

            if (btnGoToSearch != null) {
                btnGoToSearch.Dispose ();
                btnGoToSearch = null;
            }

            if (btnMyOrganizations != null) {
                btnMyOrganizations.Dispose ();
                btnMyOrganizations = null;
            }

            if (btnShare != null) {
                btnShare.Dispose ();
                btnShare = null;
            }

            if (dataLabel != null) {
                dataLabel.Dispose ();
                dataLabel = null;
            }

            if (lblEvents != null) {
                lblEvents.Dispose ();
                lblEvents = null;
            }

            if (lblMyBusidex != null) {
                lblMyBusidex.Dispose ();
                lblMyBusidex = null;
            }

            if (lblOrganizations != null) {
                lblOrganizations.Dispose ();
                lblOrganizations = null;
            }

            if (lblProgress != null) {
                lblProgress.Dispose ();
                lblProgress = null;
            }

            if (lblQuestions != null) {
                lblQuestions.Dispose ();
                lblQuestions = null;
            }

            if (lblSearch != null) {
                lblSearch.Dispose ();
                lblSearch = null;
            }
        }
    }
}