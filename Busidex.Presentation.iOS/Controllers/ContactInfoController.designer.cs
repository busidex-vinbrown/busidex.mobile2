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
    [Register ("ContactInfoController")]
    partial class ContactInfoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAddNewPhoneNumber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAddNumber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnCancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView pckPhoneNumberTypes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tblPhoneNumbers { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtNewExtension { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtNewPhoneNumber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtUrl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView vwNewPhoneNumber { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAddNewPhoneNumber != null) {
                btnAddNewPhoneNumber.Dispose ();
                btnAddNewPhoneNumber = null;
            }

            if (btnAddNumber != null) {
                btnAddNumber.Dispose ();
                btnAddNumber = null;
            }

            if (btnCancel != null) {
                btnCancel.Dispose ();
                btnCancel = null;
            }

            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (pckPhoneNumberTypes != null) {
                pckPhoneNumberTypes.Dispose ();
                pckPhoneNumberTypes = null;
            }

            if (tblPhoneNumbers != null) {
                tblPhoneNumbers.Dispose ();
                tblPhoneNumbers = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtNewExtension != null) {
                txtNewExtension.Dispose ();
                txtNewExtension = null;
            }

            if (txtNewPhoneNumber != null) {
                txtNewPhoneNumber.Dispose ();
                txtNewPhoneNumber = null;
            }

            if (txtUrl != null) {
                txtUrl.Dispose ();
                txtUrl = null;
            }

            if (vwNewPhoneNumber != null) {
                vwNewPhoneNumber.Dispose ();
                vwNewPhoneNumber = null;
            }
        }
    }
}