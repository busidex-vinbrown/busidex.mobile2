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
    [Register ("MyBusidexController")]
    partial class MyBusidexController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar SearchBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SearchBar != null) {
                SearchBar.Dispose ();
                SearchBar = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}