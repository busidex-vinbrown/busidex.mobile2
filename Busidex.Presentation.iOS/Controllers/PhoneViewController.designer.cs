// WARNING
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
    [Register ("PhoneViewController")]
    partial class PhoneViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PhoneView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgCard != null) {
                imgCard.Dispose ();
                imgCard = null;
            }

            if (PhoneView != null) {
                PhoneView.Dispose ();
                PhoneView = null;
            }
        }
    }
}