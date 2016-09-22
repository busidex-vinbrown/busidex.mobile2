using System;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;
using UIKit;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class AddressInfoController : BaseCardEditController
	{
		public AddressInfoController (IntPtr handle) : base (handle)
		{
		}

		Address address;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			const string EMPTY_STATE = "(Select State)";

			address = UnsavedData.Addresses != null && UnsavedData.Addresses.Count >= 0 ? UnsavedData.Addresses [0] : new Address ();

			txtAddress1.Text = address.Address1;
			txtAddress2.Text = address.Address2;
			txtCity.Text = address.City;
			lblSelectedState.Text = address.State != null ? address.State.Name : EMPTY_STATE;
			txtZip.Text = address.ZipCode;

			pckState.Hidden = true;

			var model = new StateCodeModel (lblSelectedState, address.State);
			model.OnItemSelected += delegate {
				pckState.Hidden = true;
				address.State = model.selectedState;
				CardInfoChanged = true;
				fadeIn ();
			};

			txtAddress1.AllEditingEvents += (sender, e) => {
				CardInfoChanged = txtAddress1.Text != address.Address1;
			};

			txtAddress2.AllEditingEvents += (sender, e) => {
				CardInfoChanged = txtAddress2.Text != address.Address2;
			};

			txtCity.AllEditingEvents += (sender, e) => {
				CardInfoChanged = txtCity.Text != address.City;
			};

			txtZip.AllEditingEvents += (sender, e) => {
				CardInfoChanged = txtZip.Text != address.ZipCode;
			};

			pckState.Model = model;

			btnPicker.TouchUpInside += delegate {
				if (address != null) {
					pckState.Select (model.IndexOf (address.State), 0, true);
				}
				pckState.Hidden = !pckState.Hidden;
				if (pckState.Hidden) {
					fadeIn ();
				} else {
					fadeOut ();
				}
				CardInfoChanged = true;
			};
		}

		public override void SaveCard ()
		{
			if (!CardInfoChanged) {
				return;
			}

			address.Address1 = txtAddress1.Text;
			address.Address2 = txtAddress2.Text;
			address.City = txtCity.Text;
			address.ZipCode = txtZip.Text;

			UnsavedData.Addresses [0] = address;
			UISubscriptionService.SaveCardInfo (new CardDetailModel (UnsavedData));

			base.SaveCard ();
		}

		void fadeOut ()
		{
			UIView.Animate (
					0.5, // duration
					() => {
						vwFields.BackgroundColor = View.BackgroundColor = UIColor.UnderPageBackgroundColor;
					},
					() => {

					}
				);
		}

		void fadeIn ()
		{
			UIView.Animate (
					0.5, // duration
					() => {
						vwFields.BackgroundColor = View.BackgroundColor = UIColor.White;
					},
					() => {

					}
				);
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Address Info");

			base.ViewDidAppear (animated);
		}
	}
}
