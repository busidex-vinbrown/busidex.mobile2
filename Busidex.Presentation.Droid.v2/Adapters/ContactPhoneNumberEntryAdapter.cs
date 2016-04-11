using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Xamarin.Contacts;


namespace Busidex.Presentation.Droid.v2
{
	public delegate void PhoneNumberSelectedHandler (Phone number);

	public class ContactPhoneNumberEntryAdapter: ArrayAdapter<Phone>
	{
		
		public event PhoneNumberSelectedHandler PhoneNumberSelected;

		List<Phone> PhoneNumbers { get; set; }

		readonly Activity context;

		public ContactPhoneNumberEntryAdapter (Activity ctx, int id, List<Phone> phoneNumbers) : base (ctx, id, phoneNumbers)
		{
			context = ctx;
			PhoneNumbers = phoneNumbers;
		}

		public override int Count {
			get {
				return PhoneNumbers.Count;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.ContactPhoneNumberEntry, null);
			var number = PhoneNumbers [position];

			var lblContactPhoneNumberType = view.FindViewById<TextView> (Resource.Id.lblContactPhoneNumberType);
			var lblContactPhoneNumber = view.FindViewById<TextView> (Resource.Id.lblContactPhoneNumber);

			lblContactPhoneNumberType.Text = number.Label + ":";
			lblContactPhoneNumber.Text = number.Number;

			view.Click += delegate {
				if (PhoneNumberSelected != null) {
					PhoneNumberSelected (number);
				}								
			};

			return view;
		}
	}
}

