using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class CardAddressInfoFragment : BaseCardEditFragment
	{
		Spinner spnState;
		EditText txtAddress1;
		EditText txtAddress2;
		EditText txtCity;
		EditText txtZip;
		Address SelectedAddress;

		public override void OnDetach ()
		{
			spnState = null;
			txtAddress1 = null;
			txtAddress2 = null;
			txtCity = null;
			txtZip = null;

			base.OnDetach ();
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_ADDRESS_INFO);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardAddressInfo, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			txtAddress1 = view.FindViewById<EditText> (Resource.Id.txtAddress1);
			txtAddress2 = view.FindViewById<EditText> (Resource.Id.txtAddress2);
			txtCity = view.FindViewById<EditText> (Resource.Id.txtCity);
			txtZip = view.FindViewById<EditText> (Resource.Id.txtZip);
			spnState = view.FindViewById<Spinner> (Resource.Id.spnState);

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				imm.HideSoftInputFromWindow (txtAddress1.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtAddress2.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtCity.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtZip.WindowToken, 0);
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			var btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {

				imm.HideSoftInputFromWindow (txtAddress1.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtAddress2.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtCity.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtZip.WindowToken, 0);

				SelectedAddress.Address1 = txtAddress1.Text;
				SelectedAddress.Address2 = txtAddress2.Text;
				SelectedAddress.City = txtCity.Text;
				SelectedAddress.ZipCode = txtZip.Text;

				SelectedCard.Addresses [0] = SelectedAddress;

				UISubscriptionService.SaveCardInfo (new CardDetailModel (SelectedCard));
			};

			SelectedAddress = SelectedCard.Addresses != null && SelectedCard.Addresses.Count > 0
										  ? SelectedCard.Addresses [0]
										  : new Address ();


			txtAddress1.Text = SelectedAddress.Address1;
			txtAddress2.Text = SelectedAddress.Address2;
			txtCity.Text = SelectedAddress.City;
			txtZip.Text = SelectedAddress.ZipCode;

			txtAddress1.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtAddress1.ClearFocus ();
				imm.HideSoftInputFromWindow (txtAddress1.WindowToken, 0);
			};
			txtAddress2.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtAddress2.ClearFocus ();
				imm.HideSoftInputFromWindow (txtAddress2.WindowToken, 0);
			};
			txtCity.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtCity.ClearFocus ();
				imm.HideSoftInputFromWindow (txtCity.WindowToken, 0);
			};
			txtZip.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtZip.ClearFocus ();
				imm.HideSoftInputFromWindow (txtZip.WindowToken, 0);
			};

			txtAddress1.FocusChange += delegate {
				btnSave.Visibility = txtAddress1.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtAddress2.FocusChange += delegate {
				btnSave.Visibility = txtAddress2.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtCity.FocusChange += delegate {
				btnSave.Visibility = txtCity.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtZip.FocusChange += delegate {
				btnSave.Visibility = txtZip.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};

			var adapter = new StateAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, UISubscriptionService.GetStates ());

			spnState.Adapter = adapter;
			if (SelectedAddress.State != null) {
				var position = UISubscriptionService.GetStates ().FindIndex ((State obj) => obj.Code == SelectedAddress.State.Code);
				spnState.SetSelection (position);
			}

			spnState.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SelectedAddress.State = ((StateAdapter)spnState.Adapter).GetItemAtPosition (e.Position);
			};
			return view;
		}
	}
}

