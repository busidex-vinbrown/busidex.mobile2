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
    [Register ("SharedCardListController")]
    partial class SharedCardListController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView vwSharedCards { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (vwSharedCards != null) {
                vwSharedCards.Dispose ();
                vwSharedCards = null;
            }
        }
    }
}