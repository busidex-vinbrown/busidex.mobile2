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
    [Register ("CardMenuController")]
    partial class CardMenuController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAddressInfo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnCardImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnContactInfo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSearchInfo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTags { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnVisibility { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAddressInfo != null) {
                btnAddressInfo.Dispose ();
                btnAddressInfo = null;
            }

            if (btnCardImage != null) {
                btnCardImage.Dispose ();
                btnCardImage = null;
            }

            if (btnContactInfo != null) {
                btnContactInfo.Dispose ();
                btnContactInfo = null;
            }

            if (btnSearchInfo != null) {
                btnSearchInfo.Dispose ();
                btnSearchInfo = null;
            }

            if (btnTags != null) {
                btnTags.Dispose ();
                btnTags = null;
            }

            if (btnVisibility != null) {
                btnVisibility.Dispose ();
                btnVisibility = null;
            }
        }
    }
}