
using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Contacts;
using Busidex.Mobile.Models;
using System.Collections;

namespace Busidex.Presentation.Droid.v2
{
	public class ContactProfileFragment : GenericViewPagerFragment
	{
		readonly Contact SelectedContact;
		readonly UserCard SelectedCard;

		public ContactProfileFragment(){
			
		}

		public ContactProfileFragment(Contact contact, UserCard card){
			SelectedContact = contact;
			SelectedCard = card;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		void phoneNumberSelected(Phone number){
			// pass the phone number to the shareCardFragment
			var card = SelectedCard;
			var fragment = new ShareCardFragment(card, number);
			((MainActivity)Activity).UnloadFragment (fragment);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.ContactProfile, container, false);

			if (SelectedContact != null) {
				var lstContactPhoneNumbers = view.FindViewById<ListView> (Resource.Id.lstContactPhoneNumbers);

				var thumbnail = view.FindViewById<ImageView> (Resource.Id.imgContact);
				if (SelectedContact.GetThumbnail () == null) {
					thumbnail.SetImageResource (Resource.Drawable.defaultprofile);
				} else {
					thumbnail.SetImageBitmap (SelectedContact.GetThumbnail ());
				}
				var name = SelectedContact.DisplayName;
				view.FindViewById<TextView> (Resource.Id.txtContactDisplayName).Text = name;

				IEqualityComparer<Phone> phoneComparer = new PhoneComparer ();
				var phoneNumbers = SelectedContact.Phones.Distinct(phoneComparer).ToList ();
				var adapter = new ContactPhoneNumberEntryAdapter (Activity, Resource.Id.lstContactPhoneNumbers, phoneNumbers);
				adapter.PhoneNumberSelected += phoneNumberSelected;

				lstContactPhoneNumbers.Adapter = adapter;

				var btnHideProfile = view.FindViewById<ImageButton> (Resource.Id.btnHideProfile);
				btnHideProfile.Click += delegate {
					var card = SelectedCard;
					var contactsAdapter = new ContactsAdapter(Activity, MainActivity.Contacts, card);
					var fragment = new ContactsFragment (contactsAdapter, card);
					((MainActivity)Activity).UnloadFragment (fragment);
				};

			}else{
				((MainActivity)Activity).UnloadFragment ();
			}
			return view;
		}


	}

	internal class PhoneComparer : IEqualityComparer<Phone>
	{
		public bool Equals(Phone x, Phone y)
		{
			return x.Number == y.Number;
		}
		public int GetHashCode(Phone p)
		{
			return p.Number.GetHashCode();
		}
	}
}

