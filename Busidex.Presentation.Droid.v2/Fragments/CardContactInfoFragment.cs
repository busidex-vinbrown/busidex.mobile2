using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class CardContactInfoFragment : BaseCardEditFragment
	{
		PhoneNumber SelectedPhoneNumber;
		ListView lstCardPhoneNumbers;
		Spinner spnPhoneNumberType;
		RelativeLayout newPhoneNumberContainer;
		EditText txtUrl;
		EditText txtEmail;

		public override void OnDetach ()
		{
			spnPhoneNumberType = null;
			lstCardPhoneNumbers = null;
			newPhoneNumberContainer = null;
			txtUrl = null;
			txtEmail = null;

			base.OnDetach ();
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CONTACT_INFO);
			}
		}

		void EditPhoneNumber (PhoneNumber number)
		{

		}

		void DeletePhoneNumber (PhoneNumber number)
		{

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardContactInfo, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			newPhoneNumberContainer = view.FindViewById<RelativeLayout> (Resource.Id.newPhoneNumberContainer);
			newPhoneNumberContainer.Visibility = ViewStates.Gone;

			spnPhoneNumberType = view.FindViewById<Spinner> (Resource.Id.spnPhoneNumberType);
			lstCardPhoneNumbers = view.FindViewById<ListView> (Resource.Id.lstCardPhoneNumbers);

			var cardPhoneNumberAdapter = new CardPhoneNumberAdapter (Activity, Resource.Id.lstCardPhoneNumbers, SelectedCard.PhoneNumbers);
			cardPhoneNumberAdapter.EditPhoneNumber += EditPhoneNumber;
			cardPhoneNumberAdapter.DeletePhoneNumber += DeletePhoneNumber;

			lstCardPhoneNumbers.Adapter = cardPhoneNumberAdapter;

			txtUrl = view.FindViewById<EditText> (Resource.Id.txtUrl);
			txtEmail = view.FindViewById<EditText> (Resource.Id.txtEmail);

			txtUrl.Text = SelectedCard.Url;
			txtEmail.Text = SelectedCard.Email;

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			var phoneNumberTypeAdapter = new PhoneNumberTypeAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, UISubscriptionService.GetPhoneNumberTypes ());

			spnPhoneNumberType.Adapter = phoneNumberTypeAdapter;
			if (SelectedPhoneNumber != null) {
				var position = UISubscriptionService.GetPhoneNumberTypes ().FindIndex ((PhoneNumberType obj) => obj.Name == SelectedPhoneNumber.PhoneNumberType.Name);
				spnPhoneNumberType.SetSelection (position);
			}

			spnPhoneNumberType.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SelectedPhoneNumber.PhoneNumberType = ((PhoneNumberTypeAdapter)spnPhoneNumberType.Adapter).GetItemAtPosition (e.Position);
			};

			return view;
		}
	}
}

