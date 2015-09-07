using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;

namespace Busidex.Presentation.Droid.v2
{

	public delegate void DialPhoneNumberHandler(PhoneNumber number);


	public class PhoneNumberEntryAdapter : ArrayAdapter<PhoneNumber>
	{
		public event DialPhoneNumberHandler PhoneNumberDialed;

		List<PhoneNumber> PhoneNumbers { get; set; }
		readonly Activity context;

		public PhoneNumberEntryAdapter (Activity ctx, int id, List<PhoneNumber> phoneNumbers) : base(ctx, id, phoneNumbers)
		{
			context = ctx;
			PhoneNumbers = phoneNumbers;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.PhoneNumberEntry, null);
			var number = PhoneNumbers [position];

			var lblPhoneNumberType = view.FindViewById<TextView> (Resource.Id.lblPhoneNumberType);
			var btnPhoneNumber = view.FindViewById<Button> (Resource.Id.btnPhoneNumber);

			lblPhoneNumberType.Text = System.Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId);
			btnPhoneNumber.Text = number.Number;

			btnPhoneNumber.Click += delegate {
				if(PhoneNumberDialed != null){
					PhoneNumberDialed(number);
				}								
			};

			return view;
		}
	}
}

