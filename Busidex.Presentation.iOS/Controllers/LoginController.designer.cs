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
    [Register ("LoginController")]
    partial class LoginController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnForgotPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnForgotUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgLogo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLoginResult { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView vwLogin { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnForgotPassword != null) {
                btnForgotPassword.Dispose ();
                btnForgotPassword = null;
            }

            if (btnForgotUserName != null) {
                btnForgotUserName.Dispose ();
                btnForgotUserName = null;
            }

            if (btnLogin != null) {
                btnLogin.Dispose ();
                btnLogin = null;
            }

            if (imgLogo != null) {
                imgLogo.Dispose ();
                imgLogo = null;
            }

            if (lblLoginResult != null) {
                lblLoginResult.Dispose ();
                lblLoginResult = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }

            if (txtUserName != null) {
                txtUserName.Dispose ();
                txtUserName = null;
            }

            if (vwLogin != null) {
                vwLogin.Dispose ();
                vwLogin = null;
            }
        }
    }
}