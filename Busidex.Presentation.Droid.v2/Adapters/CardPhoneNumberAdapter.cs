using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public delegate void EditPhoneNumberHandler (PhoneNumber number);
	public delegate void DeletePhoneNumberHandler (PhoneNumber number);

	public class CardPhoneNumberAdapter : ArrayAdapter<PhoneNumber>
	{
		public event EditPhoneNumberHandler EditPhoneNumber;
		public event DeletePhoneNumberHandler DeletePhoneNumber;

		List<PhoneNumber> PhoneNumbers { get; set; }

		readonly Activity context;

		public CardPhoneNumberAdapter (Activity ctx, int id, List<PhoneNumber> phoneNumbers) : base (ctx, id, phoneNumbers)
		{
			context = ctx;
			PhoneNumbers = phoneNumbers;
		}

		public override int Count {
			get {
				return PhoneNumbers == null ? 0 : PhoneNumbers.Count;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.CardPhoneNumber, null);
			var number = PhoneNumbers [position];

			var lblPhoneNumberType = view.FindViewById<TextView> (Resource.Id.lblPhoneNumberType);
			var lblPhoneNumber = view.FindViewById<TextView> (Resource.Id.lblPhoneNumber);
			var btnEditPhoneNumber = view.FindViewById<ImageButton> (Resource.Id.btnEditPhoneNumber);
			var btnDeletePhoneNumber = view.FindViewById<ImageButton> (Resource.Id.btnDeletePhoneNumber);

			lblPhoneNumberType.Text = System.Enum.GetName (typeof (PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId) + ":";
			lblPhoneNumber.Text = number.Number.AsPhoneNumber ();

			btnEditPhoneNumber.Click += delegate {
				if (EditPhoneNumber != null) {
					EditPhoneNumber (number);
				}
			};

			btnDeletePhoneNumber.Click += delegate {
				if (DeletePhoneNumber != null) {
					if (DeletePhoneNumber != null) {
						DeletePhoneNumber (number);
					}
				}
			};
			return view;
		}
	}
}

