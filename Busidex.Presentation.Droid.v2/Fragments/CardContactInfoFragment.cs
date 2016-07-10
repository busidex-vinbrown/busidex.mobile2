using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class CardContactInfoFragment : BaseCardEditFragment
	{
		List<PhoneNumberType> phoneNumberTypes;
		PhoneNumber SelectedPhoneNumber;
		ListView lstCardPhoneNumbers;
		Spinner spnPhoneNumberType;
		RelativeLayout newPhoneNumberContainer;
		EditText txtNewPhoneNumber;
		EditText txtExtension;
		EditText txtUrl;
		EditText txtEmail;
		ImageButton btnAddPhoneNumber;
		Button btnCancel;
		Button btnSavePhoneNumber;
		Button btnSave;

		public override void OnDetach ()
		{
			spnPhoneNumberType = null;
			lstCardPhoneNumbers = null;
			newPhoneNumberContainer = null;
			txtUrl = null;
			txtEmail = null;
			btnAddPhoneNumber = null;
			txtNewPhoneNumber = null;
			txtExtension = null;
			btnSave = null;

			UISubscriptionService.OnCardInfoSaved -= AfterCardUpdated;

			base.OnDetach ();
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CONTACT_INFO);
			}
		}

		void CancelEdit ()
		{
			newPhoneNumberContainer.Visibility = updateCover.Visibility = ViewStates.Gone;
			txtNewPhoneNumber.Text = string.Empty;
			txtExtension.Text = string.Empty;
		}

		void populatePhoneFields (PhoneNumber number)
		{
			txtNewPhoneNumber.Text = number.Number;
			txtExtension.Text = number.Extension;
		}

		void EditPhoneNumber (PhoneNumber number)
		{
			SelectedPhoneNumber = number;
			newPhoneNumberContainer.Visibility = updateCover.Visibility = ViewStates.Visible;
			btnSavePhoneNumber.Text = "Update Phone Number";
			populatePhoneFields (SelectedPhoneNumber);
			setPhoneNumberType ();
		}

		void AddPhoneNumber ()
		{
			newPhoneNumberContainer.Visibility = updateCover.Visibility = ViewStates.Visible;
			btnSavePhoneNumber.Text = "Add Phone Number";
			SelectedPhoneNumber = new PhoneNumber ();
			SelectedPhoneNumber.PhoneNumberType = new PhoneNumberType ();

			populatePhoneFields (SelectedPhoneNumber);
			setPhoneNumberType ();
		}

		void SavePhoneNumber ()
		{
			if (SelectedPhoneNumber.PhoneNumberId > 0) {
				SelectedCard.PhoneNumbers.ForEach ((PhoneNumber p) => {
					if (p.PhoneNumberId == SelectedPhoneNumber.PhoneNumberId) {
						p.Number = SelectedPhoneNumber.Number;
						p.Extension = SelectedPhoneNumber.Extension;
						p.PhoneNumberType = SelectedPhoneNumber.PhoneNumberType;
						p.PhoneNumberTypeId = SelectedPhoneNumber.PhoneNumberTypeId;
					}
				});
			} else {
				SelectedCard.PhoneNumbers.Add (new PhoneNumber {
					Number = txtNewPhoneNumber.Text,
					Extension = txtExtension.Text,
					PhoneNumberType = SelectedPhoneNumber.PhoneNumberType,
					PhoneNumberTypeId = SelectedPhoneNumber.PhoneNumberType.PhoneNumberTypeId
				});
			}
			resetAdapter ();

			SelectedPhoneNumber = new PhoneNumber ();
			SelectedPhoneNumber.PhoneNumberType = new PhoneNumberType ();
			populatePhoneFields (SelectedPhoneNumber);
			CancelEdit ();
		}

		void DeletePhoneNumber (PhoneNumber number)
		{
			ShowAlert ("Delete", "Delete " + number.Number, GetString (Resource.String.Global_ButtonText_Ok),
					  new System.EventHandler<DialogClickEventArgs> ((o, e) => {

						  var dialog = o as global::Android.App.AlertDialog;
						  var btnClicked = dialog.GetButton (e.Which);
						  if (btnClicked.Text == Activity.GetString (Resource.String.Global_ButtonText_Ok)) {
							  var idx = SelectedCard.PhoneNumbers.FindIndex (p => p.Number == number.Number && p.PhoneNumberType.Name == number.PhoneNumberType.Name);
							  if (idx >= 0) {
								  SelectedCard.PhoneNumbers [idx].Deleted = true;
								  resetAdapter ();
							  }
						  }
					  }));
		}

		void resetAdapter ()
		{
			var cardPhoneNumberAdapter = new CardPhoneNumberAdapter (Activity, Resource.Id.lstCardPhoneNumbers, SelectedCard.PhoneNumbers.Where (p => !p.Deleted).ToList ());
			cardPhoneNumberAdapter.EditPhoneNumber -= EditPhoneNumber;
			cardPhoneNumberAdapter.DeletePhoneNumber -= DeletePhoneNumber;

			cardPhoneNumberAdapter.EditPhoneNumber += EditPhoneNumber;
			cardPhoneNumberAdapter.DeletePhoneNumber += DeletePhoneNumber;

			lstCardPhoneNumbers.Adapter = cardPhoneNumberAdapter;
		}

		void setPhoneNumberType ()
		{
			var position = phoneNumberTypes.FindIndex ((PhoneNumberType obj) => obj.Name == SelectedPhoneNumber.PhoneNumberType.Name);
			spnPhoneNumberType.SetSelection (position);
		}

		void AfterCardUpdated ()
		{
			Activity.RunOnUiThread (() => {
				resetAdapter ();
			});
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardContactInfo, container, false);

			phoneNumberTypes = UISubscriptionService.GetPhoneNumberTypes ();

			base.OnCreateView (inflater, container, savedInstanceState);

			UISubscriptionService.OnCardInfoSaved += AfterCardUpdated;

			newPhoneNumberContainer = view.FindViewById<RelativeLayout> (Resource.Id.newPhoneNumberContainer);
			newPhoneNumberContainer.Visibility = ViewStates.Gone;

			updateCover = view.FindViewById<RelativeLayout> (Resource.Id.updateCover);
			updateCover.Visibility = ViewStates.Gone;

			spnPhoneNumberType = view.FindViewById<Spinner> (Resource.Id.spnPhoneNumberType);
			lstCardPhoneNumbers = view.FindViewById<ListView> (Resource.Id.lstCardPhoneNumbers);

			resetAdapter ();

			txtUrl = view.FindViewById<EditText> (Resource.Id.txtUrl);
			txtEmail = view.FindViewById<EditText> (Resource.Id.txtEmail);
			txtNewPhoneNumber = view.FindViewById<EditText> (Resource.Id.txtNewPhoneNumber);
			txtExtension = view.FindViewById<EditText> (Resource.Id.txtExtension);

			txtUrl.Text = SelectedCard.Url;
			txtEmail.Text = SelectedCard.Email;

			txtNewPhoneNumber.AddTextChangedListener (new PhoneNumberFormattingTextWatcher ());


			btnAddPhoneNumber = view.FindViewById<ImageButton> (Resource.Id.btnAddPhoneNumber);
			btnAddPhoneNumber.Click += delegate {
				AddPhoneNumber ();
			};

			btnSavePhoneNumber = view.FindViewById<Button> (Resource.Id.btnSavePhoneNumber);
			btnSavePhoneNumber.Click += delegate {
				SavePhoneNumber ();
				imm.HideSoftInputFromWindow (txtNewPhoneNumber.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtExtension.WindowToken, 0);
			};

			btnCancel = view.FindViewById<Button> (Resource.Id.btnCancel);
			btnCancel.Click += delegate {
				CancelEdit ();
			};

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {
				imm.HideSoftInputFromWindow (txtUrl.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtEmail.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtNewPhoneNumber.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtExtension.WindowToken, 0);

				SelectedCard.Url = txtUrl.Text;
				SelectedCard.Email = txtEmail.Text;
				UISubscriptionService.SaveCardInfo (new CardDetailModel (SelectedCard));
			};

			var phoneNumberTypeAdapter = new PhoneNumberTypeAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, UISubscriptionService.GetPhoneNumberTypes ());

			SelectedPhoneNumber = SelectedCard.PhoneNumbers.FirstOrDefault ();

			spnPhoneNumberType.Adapter = phoneNumberTypeAdapter;
			if (SelectedPhoneNumber != null) {
				setPhoneNumberType ();
			}

			spnPhoneNumberType.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				var newPhoneType = ((PhoneNumberTypeAdapter)spnPhoneNumberType.Adapter).GetItemAtPosition (e.Position);
				SelectedPhoneNumber.PhoneNumberType = newPhoneType;
			};

			imm.HideSoftInputFromWindow (txtUrl.WindowToken, 0);
			imm.HideSoftInputFromWindow (txtEmail.WindowToken, 0);
			imm.HideSoftInputFromWindow (txtNewPhoneNumber.WindowToken, 0);
			imm.HideSoftInputFromWindow (txtExtension.WindowToken, 0);

			txtUrl.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtUrl.ClearFocus ();
				imm.HideSoftInputFromWindow (txtUrl.WindowToken, 0);
			};
			txtEmail.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtEmail.ClearFocus ();
				imm.HideSoftInputFromWindow (txtEmail.WindowToken, 0);
			};
			txtNewPhoneNumber.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtNewPhoneNumber.ClearFocus ();
				imm.HideSoftInputFromWindow (txtNewPhoneNumber.WindowToken, 0);
			};
			txtExtension.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtExtension.ClearFocus ();
				imm.HideSoftInputFromWindow (txtExtension.WindowToken, 0);
			};

			txtUrl.FocusChange += delegate {
				btnSave.Visibility = txtUrl.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtEmail.FocusChange += delegate {
				btnSave.Visibility = txtEmail.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtNewPhoneNumber.FocusChange += delegate {
				btnSave.Visibility = txtNewPhoneNumber.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtExtension.FocusChange += delegate {
				btnSave.Visibility = txtExtension.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			return view;
		}
	}
}

