using System;
using ContactsUI;
using Contacts;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class ContactPickerDelegate: CNContactPickerDelegate
	{
		UITextField txtPhoneNumber;

		#region Constructors
		public ContactPickerDelegate ()
		{
		}

		public ContactPickerDelegate (UITextField txt)
		{
			txtPhoneNumber = txt;
		}

		public ContactPickerDelegate (IntPtr handle) : base (handle)
		{
		}

		#endregion

		#region Override Methods
		public override void ContactPickerDidCancel (CNContactPickerViewController picker)
		{
			//Console.WriteLine ("User canceled picker");

		}

		public override void DidSelectContact (CNContactPickerViewController picker, CNContact contact)
		{
			//Console.WriteLine ("Selected: {0}", contact);
//			if(contact.PhoneNumbers.GetLength() == 1){
//				var phoneNumber = contact.PhoneNumbers [0].Value as CNPhoneNumber;
//				if(phoneNumber != null){
//					txtPhoneNumber.Text = phoneNumber.StringValue;
//				}
//			}
		}

		public override void DidSelectContactProperty (CNContactPickerViewController picker, CNContactProperty contactProperty)
		{
			var phoneNumberObject = contactProperty.Value as CNPhoneNumber;
			if (phoneNumberObject != null) {
				txtPhoneNumber.Text = phoneNumberObject.StringValue;
			}
			//Console.WriteLine ("Selected Property: {0}", contactProperty.Value);
		}
		#endregion
	}
}

