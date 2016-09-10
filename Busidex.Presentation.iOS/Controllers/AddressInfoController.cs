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

			address = SelectedCard.Addresses != null && SelectedCard.Addresses.Count >= 0 ? SelectedCard.Addresses [0] : new Address ();

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
				fadeIn ();
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
			};
		}

		public override void SaveCard ()
		{
			address.Address1 = txtAddress1.Text;
			address.Address2 = txtAddress2.Text;
			address.City = txtCity.Text;
			address.ZipCode = txtZip.Text;

			SelectedCard.Addresses [0] = address;
			UISubscriptionService.SaveCardInfo (new CardDetailModel (SelectedCard));

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
