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
    [Register ("NotesController")]
    partial class NotesController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSaveNotes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgSaved { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtNotes { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnSaveNotes != null) {
                btnSaveNotes.Dispose ();
                btnSaveNotes = null;
            }

            if (imgCard != null) {
                imgCard.Dispose ();
                imgCard = null;
            }

            if (imgSaved != null) {
                imgSaved.Dispose ();
                imgSaved = null;
            }

            if (txtNotes != null) {
                txtNotes.Dispose ();
                txtNotes = null;
            }
        }
    }
}