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
    [Register ("TermsController")]
    partial class TermsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPrivacy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView vwTerms { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPrivacy != null) {
                btnPrivacy.Dispose ();
                btnPrivacy = null;
            }

            if (vwTerms != null) {
                vwTerms.Dispose ();
                vwTerms = null;
            }
        }
    }
}