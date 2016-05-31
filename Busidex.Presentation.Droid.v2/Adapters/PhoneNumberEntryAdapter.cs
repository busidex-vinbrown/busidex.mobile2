using Android.Widget;
using Android.Views;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;

namespace Busidex.Presentation.Droid.v2
{

	public delegate void DialPhoneNumberHandler (PhoneNumber number);
	public delegate void SendTextMessageHandler (PhoneNumber number);

	public class PhoneNumberEntryAdapter : ArrayAdapter<PhoneNumber>
	{
		public event DialPhoneNumberHandler PhoneNumberDialed;
		public event SendTextMessageHandler TextMessageSent;

		List<PhoneNumber> PhoneNumbers { get; set; }

		readonly Activity context;

		public PhoneNumberEntryAdapter (Activity ctx, int id, List<PhoneNumber> phoneNumbers) : base (ctx, id, phoneNumbers)
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
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.PhoneNumberEntry, null);
			var number = PhoneNumbers [position];

			var lblPhoneNumberType = view.FindViewById<TextView> (Resource.Id.lblPhoneNumberType);
			var lblPhoneNumber = view.FindViewById<TextView> (Resource.Id.lblPhoneNumber);
			var btnPhoneNumber = view.FindViewById<ImageButton> (Resource.Id.btnPhoneNumber);
			var btnTextMessage = view.FindViewById<ImageButton> (Resource.Id.btnTextMessage);

			lblPhoneNumberType.Text = System.Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId) + ":";
			lblPhoneNumber.Text = number.Number;

			btnPhoneNumber.Click += delegate {
				if (PhoneNumberDialed != null) {
					PhoneNumberDialed (number);
				}								
			};

			btnTextMessage.Click += delegate {
				if (TextMessageSent != null) {
					TextMessageSent (number);
				}								
			};
			return view;
		}
	}
}

