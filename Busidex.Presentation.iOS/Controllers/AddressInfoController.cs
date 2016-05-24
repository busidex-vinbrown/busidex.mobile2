using System;

using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public partial class AddressInfoController : BaseCardEditController
	{
		public AddressInfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			const string EMPTY_STATE = "(Select State)";

			var address = SelectedCard.Addresses != null && SelectedCard.Addresses.Count >= 0 ? SelectedCard.Addresses [0] : new Address ();

			txtAddress1.Text = address.Address1;
			txtAddress2.Text = address.Address2;
			txtCity.Text = address.City;
			lblSelectedState.Text = address.State != null ? address.State.Name : EMPTY_STATE;
			txtZip.Text = address.ZipCode;

			pckState.Hidden = true;

			var model = new StateCodeModel (lblSelectedState, address.State);
			model.OnItemSelected += delegate {
				pckState.Hidden = true;
			};

			pckState.Model = model;

			btnPicker.TouchUpInside += delegate {
				if (address != null) {
					pckState.Select (model.IndexOf (address.State), 0, true);
				}
				pckState.Hidden = !pckState.Hidden;
			};
			vwFields.SetContentOffset (new CoreGraphics.CGPoint (0, 50), false);
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Address Info");

			base.ViewDidAppear (animated);
		}




	}
}
