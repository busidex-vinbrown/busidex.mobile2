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
    [Register ("AddressInfoController")]
    partial class AddressInfoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSelectedState { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView pckState { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtAddress1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtAddress2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtCity { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtZip { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView vwAddress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TPKeyboardAvoiding.TPKeyboardAvoidingScrollView vwFields { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPicker != null) {
                btnPicker.Dispose ();
                btnPicker = null;
            }

            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (lblSelectedState != null) {
                lblSelectedState.Dispose ();
                lblSelectedState = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (pckState != null) {
                pckState.Dispose ();
                pckState = null;
            }

            if (txtAddress1 != null) {
                txtAddress1.Dispose ();
                txtAddress1 = null;
            }

            if (txtAddress2 != null) {
                txtAddress2.Dispose ();
                txtAddress2 = null;
            }

            if (txtCity != null) {
                txtCity.Dispose ();
                txtCity = null;
            }

            if (txtZip != null) {
                txtZip.Dispose ();
                txtZip = null;
            }

            if (vwAddress != null) {
                vwAddress.Dispose ();
                vwAddress = null;
            }

            if (vwFields != null) {
                vwFields.Dispose ();
                vwFields = null;
            }
        }
    }
}