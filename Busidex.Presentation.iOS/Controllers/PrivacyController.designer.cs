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
    [Register ("PrivacyController")]
    partial class PrivacyController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView vwPrivacy { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnTerms != null) {
                btnTerms.Dispose ();
                btnTerms = null;
            }

            if (vwPrivacy != null) {
                vwPrivacy.Dispose ();
                vwPrivacy = null;
            }
        }
    }
}