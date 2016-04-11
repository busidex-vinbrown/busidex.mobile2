
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Contacts;

namespace Busidex.Presentation.Droid.v2
{
	public class ContactProfileFragment : GenericViewPagerFragment
	{
		readonly Contact SelectedContact;

		public ContactProfileFragment(){
			
		}

		public ContactProfileFragment(Contact contact){
			SelectedContact = contact;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.ContactProfile, container, false);

			var lstContactPhoneNumbers = view.FindViewById<ListView> (Resource.Id.lstContactPhoneNumbers);

			var thumbnail = view.FindViewById<ImageView> (Resource.Id.imgContact);
			if(SelectedContact.GetThumbnail() == null){
				thumbnail.Visibility = ViewStates.Gone;
			}else{
				thumbnail.SetImageBitmap (SelectedContact.GetThumbnail ());
				thumbnail.Visibility = ViewStates.Visible;
			}
			var name = SelectedContact.DisplayName;
			view.FindViewById<TextView> (Resource.Id.txtContactDisplayName).Text = name;

			var phoneNumbers = SelectedContact.Phones.ToList ();
			var adapter = new ContactPhoneNumberEntryAdapter (Activity, Resource.Id.lstContactPhoneNumbers, phoneNumbers);
			lstContactPhoneNumbers.Adapter = adapter;

			return view;
		}
	}
}

